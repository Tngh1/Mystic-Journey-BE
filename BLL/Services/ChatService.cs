using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BLL.Services
{
    // Executes core business logic for i chat service.
    public class ChatService : IChatService
    {
        private const int MaxCachedMessages = 100;
        private const int MaxCachedWorldMessages = 100;
        private static readonly TimeSpan WorldSendCooldown = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan ConversationCacheTtl = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan WorldCacheTtl = TimeSpan.FromMinutes(5);
        private static readonly ConcurrentDictionary<int, SemaphoreSlim> WorldSenderLocks = new();
        private static readonly JsonSerializerOptions CacheJsonOptions = new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly IChatMessageRepository _repository;
        private readonly IPlayerProfileRepository _playerProfileRepository;
        private readonly IFriendRepository _friendRepository;
        private readonly IChatModerationService _moderationService;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;

        // Initialize this instance from repository, player profile repository, friend repository, and moderation service and store repository, player profile repository, friend repository, moderation service, and mapper for later operations.
        public ChatService(
            IChatMessageRepository repository,
            IPlayerProfileRepository playerProfileRepository,
            IFriendRepository friendRepository,
            IChatModerationService moderationService,
            IMapper mapper,
            IDistributedCache cache)
        {
            _repository = repository;
            _playerProfileRepository = playerProfileRepository;
            _friendRepository = friendRepository;
            _moderationService = moderationService;
            _mapper = mapper;
            _cache = cache;
        }

        // Load world messages using player profile id and query; it loads cache value, loads world cache key, materializes the query results, loads world messages paged, and builds map and guards invalid or unavailable states.
        public async Task<PagedResultDto<WorldChatMessageResponseDto>> GetWorldMessages(
            int playerProfileId,
            WorldChatMessageListQueryDto query)
        {
            if (playerProfileId <= 0)
                throw new UnauthorizedAccessException("Player profile not found.");  // Authentication token is invalid or expired

            var page = Math.Max(1, query.Page);
            // Clamp the calculated value to the minimum and maximum accepted by this domain rule.
            var pageSize = Math.Clamp(query.PageSize, 1, MaxCachedWorldMessages);

            await EnsurePlayerExists(playerProfileId, "Player profile not found.");

            if (page == 1)
            {
                var cached = await GetCacheValue<CachedWorldMessages>(GetWorldCacheKey());
                if (cached != null)  // Entity exists — proceed with conditional branch
                {
                    var cachedItems = cached.Items
                        .TakeLast(pageSize)
                        .ToList();

                    return new PagedResultDto<WorldChatMessageResponseDto>(cached.TotalCount, cachedItems);
                }
            }

            var repositoryPageSize = page == 1 ? MaxCachedWorldMessages : pageSize;
            var (totalCount, messages) = await _repository.GetWorldMessagesPaged(page, repositoryPageSize);

            var dtos = _mapper.Map<List<WorldChatMessageResponseDto>>(messages);  // Transform domain entity into DTO for the API response layer
            dtos.Reverse();

            if (page == 1)
            {
                await CacheWorldMessages(totalCount, dtos);
                dtos = dtos.TakeLast(pageSize).ToList();
            }

            return new PagedResultDto<WorldChatMessageResponseDto>(totalCount, dtos);
        }

        // Process the supplied values: maps the input discriminator to the corresponding domain value and fallback.
        public async Task<WorldChatMessageResponseDto> SendWorldMessage(
            int senderId,
            SendWorldChatMessageRequestDto request)
        {
            if (senderId <= 0)
                throw new UnauthorizedAccessException("Player profile not found.");  // Authentication token is invalid or expired

            var content = NormalizeContent(request.Content);
            await EnsurePlayerExists(senderId, "Player profile not found.");
            await _moderationService.EnsureCanSendChat(senderId);

            var senderLock = WorldSenderLocks.GetOrAdd(senderId, _ => new SemaphoreSlim(1, 1));
            await senderLock.WaitAsync();

            try
            {
                var now = DateTime.UtcNow;
                var rateLimitKey = GetWorldRateLimitCacheKey(senderId);
                var cachedLastSentAt = await GetCachedDateTime(rateLimitKey);

                if (cachedLastSentAt.HasValue)
                {
                    ThrowIfCooldownActive(cachedLastSentAt.Value, now, WorldSendCooldown);
                }

                var lastSentAt = await _repository.GetLatestWorldSentAtBySenderId(senderId);
                if (lastSentAt.HasValue)
                {
                    ThrowIfCooldownActive(lastSentAt.Value, now, WorldSendCooldown);
                }

                var message = _mapper.Map<WorldChatMessage>(request);  // Transform domain entity into DTO for the API response layer
                message.SenderId = senderId;
                message.Content = content;
                message.IsReported = false;
                message.IsHidden = false;
                message.SentAt = now;

                var created = await _repository.CreateWorldMessage(message);
                var dto = _mapper.Map<WorldChatMessageResponseDto>(created);  // Transform domain entity into DTO for the API response layer

                await SetCachedDateTime(rateLimitKey, dto.SentAt, WorldSendCooldown);
                await AppendToWorldCache(dto);

                return dto;
            }
            finally
            {
                senderLock.Release();
            }
        }

        // Process report world message using reporter id and request; it loads world message by id, updates world message, and builds map and guards invalid or unavailable states.
        public async Task<ReportWorldChatMessageResponseDto> ReportWorldMessage(
            int reporterId,
            ReportChatMessageRequestDto request)
        {
            if (reporterId <= 0)
                throw new UnauthorizedAccessException("Player profile not found.");  // Authentication token is invalid or expired

            if (request.ChatMessageId <= 0)
                throw new BadRequestException("Chat message ID must be greater than 0.");  // Business rule violation — surface as 400 Bad Request

            var reason = NormalizeOptionalText(
                request.Reason,
                500,
                "Reason must not exceed 500 characters.");

            await EnsurePlayerExists(reporterId, "Player profile not found.");

            var message = await _repository.GetWorldMessageById(request.ChatMessageId);
            if (message == null)  // Entity not found — short-circuit with appropriate error result
                throw new KeyNotFoundException("World chat message not found.");

            if (message.SenderId == reporterId)
                throw new BadRequestException("You cannot report your own message.");  // Business rule violation — surface as 400 Bad Request

            message.IsReported = true;
            message.ReportedById = reporterId;
            message.ReportReason = reason;
            message.ReportedAt = DateTime.UtcNow;

            var updated = await _repository.UpdateWorldMessage(message);
            var dto = _mapper.Map<WorldChatMessageResponseDto>(updated);  // Transform domain entity into DTO for the API response layer
            var moderation = await _moderationService.ReviewReportedWorldMessage(reporterId, updated, reason);

            await ReplaceWorldCacheMessage(dto);

            return new ReportWorldChatMessageResponseDto
            {
                Message = dto,
                Moderation = moderation
            };
        }

        public async Task<ChatModerationResultDto> ReportPartyMessage(
            int reporterId,
            ReportPartyChatMessageRequestDto request)
        {
            if (reporterId <= 0)
                throw new UnauthorizedAccessException("Player profile not found.");

            if (request == null)
                throw new BadRequestException("Report payload is required.");

            if (request.ReportedPlayerId <= 0)
                throw new BadRequestException("Reported player ID must be greater than 0.");

            if (request.ReportedPlayerId == reporterId)
                throw new BadRequestException("You cannot report your own message.");

            string content = request.Content?.Trim() ?? string.Empty;
            if (content.Length == 0 || content.Length > 500)
                throw new BadRequestException("Content must be between 1 and 500 characters.");

            string? reason = NormalizeOptionalText(
                request.Reason,
                500,
                "Reason must not exceed 500 characters.");

            await EnsurePlayerExists(reporterId, "Player profile not found.");
            await EnsurePlayerExists(request.ReportedPlayerId, "Reported player profile not found.");

            return await _moderationService.ReviewReportedPartyMessage(
                reporterId,
                request.ReportedPlayerId,
                content,
                reason);
        }
        // Load messages using player profile id and query; it loads conversation cache key, loads cache value, materializes the query results, and loads conversation paged and guards invalid or unavailable states.
        public async Task<PagedResultDto<ChatMessageResponseDto>> GetMessages(
            int playerProfileId,
            ChatMessageListQueryDto query)
        {
            if (playerProfileId <= 0)
                throw new UnauthorizedAccessException("Player profile not found.");  // Authentication token is invalid or expired

            var recipientId = query.RecipientId;
            if (recipientId <= 0)
                throw new BadRequestException("Recipient ID must be greater than 0.");  // Business rule violation — surface as 400 Bad Request

            if (playerProfileId == recipientId)
                throw new BadRequestException("You cannot open a chat with yourself.");  // Business rule violation — surface as 400 Bad Request

            var page = Math.Max(1, query.Page);
            // Clamp the calculated value to the minimum and maximum accepted by this domain rule.
            var pageSize = Math.Clamp(query.PageSize, 1, MaxCachedMessages);

            await EnsurePlayerExists(playerProfileId, "Player profile not found.");
            await EnsurePlayerExists(recipientId, "Recipient profile not found.");
            await EnsureCanChatWithFriend(playerProfileId, recipientId);

            if (page == 1 && pageSize <= MaxCachedMessages)
            {
                var cacheKey = GetConversationCacheKey(playerProfileId, recipientId);
                var cached = await GetCacheValue<CachedConversation>(cacheKey);
                if (cached != null)  // Entity exists — proceed with conditional branch
                {
                    var cachedItems = cached.Items
                        .TakeLast(pageSize)
                        .ToList();

                    return new PagedResultDto<ChatMessageResponseDto>(cached.TotalCount, cachedItems);
                }
            }

            var repositoryPageSize = page == 1 ? MaxCachedMessages : pageSize;
            var (totalCount, messages) = await _repository.GetConversationPaged(
                playerProfileId,
                recipientId,
                page,
                repositoryPageSize);

            var dtos = _mapper.Map<List<ChatMessageResponseDto>>(messages);  // Transform domain entity into DTO for the API response layer
            dtos.Reverse();

            if (page == 1)
            {
                await CacheConversation(playerProfileId, recipientId, totalCount, dtos);
                dtos = dtos.TakeLast(pageSize).ToList();
            }

            return new PagedResultDto<ChatMessageResponseDto>(totalCount, dtos);
        }

        // Process message using sender id and request; it builds map and creates create and guards invalid or unavailable states.
        public async Task<ChatMessageResponseDto> SendMessage(
            int senderId,
            SendChatMessageRequestDto request)
        {
            if (senderId <= 0)
                throw new UnauthorizedAccessException("Player profile not found.");  // Authentication token is invalid or expired

            if (request.RecipientId <= 0)
                throw new BadRequestException("Recipient ID must be greater than 0.");  // Business rule violation — surface as 400 Bad Request

            if (senderId == request.RecipientId)
                throw new BadRequestException("You cannot send a message to yourself.");  // Business rule violation — surface as 400 Bad Request

            var content = NormalizeContent(request.Content);

            await EnsurePlayerExists(senderId, "Player profile not found.");
            await _moderationService.EnsureCanSendChat(senderId);
            await EnsurePlayerExists(request.RecipientId, "Recipient profile not found.");
            await EnsureCanChatWithFriend(senderId, request.RecipientId);

            var now = DateTime.UtcNow;
            var message = _mapper.Map<ChatMessage>(request);  // Transform domain entity into DTO for the API response layer
            message.SenderId = senderId;
            message.RecipientId = request.RecipientId;
            message.Content = content;
            message.IsReported = false;
            message.IsHidden = false;
            message.SentAt = now;

            var created = await _repository.Create(message);
            var dto = _mapper.Map<ChatMessageResponseDto>(created);  // Transform domain entity into DTO for the API response layer

            await AppendToConversationCache(senderId, request.RecipientId, dto);

            return dto;
        }

        // Process report message using reporter id and request; it loads message by id, updates update, and builds map and guards invalid or unavailable states.
        public async Task<ReportChatMessageResponseDto> ReportMessage(
            int reporterId,
            ReportChatMessageRequestDto request)
        {
            if (reporterId <= 0)
                throw new UnauthorizedAccessException("Player profile not found.");  // Authentication token is invalid or expired

            if (request.ChatMessageId <= 0)
                throw new BadRequestException("Chat message ID must be greater than 0.");  // Business rule violation — surface as 400 Bad Request

            var reason = NormalizeOptionalText(
                request.Reason,
                500,
                "Reason must not exceed 500 characters.");

            await EnsurePlayerExists(reporterId, "Player profile not found.");

            var message = await _repository.GetMessageById(request.ChatMessageId);
            if (message == null)  // Entity not found — short-circuit with appropriate error result
                throw new KeyNotFoundException("Chat message not found.");

            if (message.SenderId != reporterId && message.RecipientId != reporterId)
                throw new BadRequestException("You can only report messages in your conversation.");  // Business rule violation — surface as 400 Bad Request

            if (message.SenderId == reporterId)
                throw new BadRequestException("You cannot report your own message.");  // Business rule violation — surface as 400 Bad Request

            message.IsReported = true;
            message.ReportedById = reporterId;
            message.ReportReason = reason;
            message.ReportedAt = DateTime.UtcNow;

            var updated = await _repository.Update(message);
            var dto = _mapper.Map<ChatMessageResponseDto>(updated);  // Transform domain entity into DTO for the API response layer
            var moderation = await _moderationService.ReviewReportedMessage(reporterId, updated, reason);

            await ReplaceConversationCacheMessage(dto);

            return new ReportChatMessageResponseDto
            {
                Message = dto,
                Moderation = moderation
            };
        }
        // Executes core business logic for ensure player exists.
        // Logic details: delegates data queries and updates to repository layer; throws BadRequestException, KeyNotFoundException on invalid state or rule violations.
        // Completes asynchronously upon successful execution.
        private async Task EnsurePlayerExists(int playerProfileId, string message)
        {
            var player = await _playerProfileRepository.GetPlayerProfileById(playerProfileId);
            if (player == null)  // Entity not found — short-circuit with appropriate error result
                throw new KeyNotFoundException(message);
        }

        // Executes core business logic for ensure can chat with friend.
        // Logic details: validates required non-empty string arguments; delegates data queries and updates to repository layer; throws BadRequestException on invalid state or rule violations.
        // Completes asynchronously upon successful execution.
        private async Task EnsureCanChatWithFriend(int playerProfileId, int friendProfileId)
        {
            var block = await _friendRepository.GetFriendBlock(playerProfileId, friendProfileId);
            var reverseBlock = await _friendRepository.GetFriendBlock(friendProfileId, playerProfileId);
            if (block != null || reverseBlock != null)
                throw new BadRequestException("Cannot chat with this player.");  // Business rule violation — surface as 400 Bad Request

            var friendship = await _friendRepository.GetFriendship(playerProfileId, friendProfileId);
            if (friendship == null || friendship.Status != "Accepted")
                throw new BadRequestException("You can only chat with accepted friends.");  // Business rule violation — surface as 400 Bad Request
        }

        // Executes core business logic for normalize content.
        // Logic details: validates required non-empty string arguments; throws BadRequestException on invalid state or rule violations.
        private static string NormalizeContent(string? content)
        {
            var normalized = content?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(normalized))  // Mandatory string argument is blank — fail fast
                throw new BadRequestException("Content is required.");  // Business rule violation — surface as 400 Bad Request

            if (normalized.Length > 500)
                throw new BadRequestException("Content must not exceed 500 characters.");  // Business rule violation — surface as 400 Bad Request

            return normalized;
        }

        // Executes core business logic for normalize optional text.
        // Logic details: validates required non-empty string arguments; throws BadRequestException, ChatRateLimitException on invalid state or rule violations.
        private static string? NormalizeOptionalText(string? value, int maxLength, string errorMessage)
        {
            var normalized = value?.Trim();
            if (string.IsNullOrWhiteSpace(normalized))  // Mandatory string argument is blank — fail fast
                return null;

            if (normalized.Length > maxLength)
                throw new BadRequestException(errorMessage);  // Business rule violation — surface as 400 Bad Request

            return normalized;
        }

        // Executes core business logic for throw if cooldown active.
        // Logic details: throws ChatRateLimitException on invalid state or rule violations.
        private static void ThrowIfCooldownActive(DateTime lastSentAt, DateTime now, TimeSpan cooldown)
        {
            var remaining = cooldown - (now - lastSentAt);
            if (remaining > TimeSpan.Zero)
            {
                throw new ChatRateLimitException((int)Math.Ceiling(remaining.TotalSeconds));
            }
        }

        // Executes core business logic for cache world messages.
        // Completes asynchronously upon successful execution.
        private async Task CacheWorldMessages(int totalCount, List<WorldChatMessageResponseDto> items)
        {
            var cached = new CachedWorldMessages
            {
                TotalCount = totalCount,
                Items = items.TakeLast(MaxCachedWorldMessages).ToList()
            };

            await SetCacheValue(GetWorldCacheKey(), cached, WorldCacheTtl);
        }

        // Executes core business logic for append to world cache.
        // Completes asynchronously upon successful execution.
        private async Task AppendToWorldCache(WorldChatMessageResponseDto message)
        {
            var cacheKey = GetWorldCacheKey();
            var cached = await GetCacheValue<CachedWorldMessages>(cacheKey);
            if (cached == null)  // Entity not found — short-circuit with appropriate error result
            {
                await RefreshWorldCacheFromDatabase();
                return;
            }

            cached.Items.Add(message);
            cached.TotalCount += 1;
            cached.Items = cached.Items
                .OrderBy(x => x.SentAt)  // Sort results oldest/lowest first
                .ThenBy(x => x.ChatMessageId)
                .TakeLast(MaxCachedWorldMessages)
                .ToList();

            await SetCacheValue(cacheKey, cached, WorldCacheTtl);
        }

        // Executes core business logic for replace world cache message.
        // Logic details: validates numeric boundary constraints; delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models.
        // Completes asynchronously upon successful execution.
        private async Task ReplaceWorldCacheMessage(WorldChatMessageResponseDto message)
        {
            var cacheKey = GetWorldCacheKey();
            var cached = await GetCacheValue<CachedWorldMessages>(cacheKey);
            if (cached == null)  // Entity not found — short-circuit with appropriate error result
                return;

            var index = cached.Items.FindIndex(x => x.ChatMessageId == message.ChatMessageId);
            if (index < 0)
                return;

            cached.Items[index] = message;
            await SetCacheValue(cacheKey, cached, WorldCacheTtl);
        }

        // Executes core business logic for refresh world cache from database.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models.
        // Completes asynchronously upon successful execution.
        private async Task RefreshWorldCacheFromDatabase()
        {
            var (totalCount, messages) = await _repository.GetWorldMessagesPaged(1, MaxCachedWorldMessages);
            var dtos = _mapper.Map<List<WorldChatMessageResponseDto>>(messages);  // Transform domain entity into DTO for the API response layer
            dtos.Reverse();
            await CacheWorldMessages(totalCount, dtos);
        }

        // Executes core business logic for get world cache key.
        private static string GetWorldCacheKey()
            => "chat:world:latest";

        // Executes core business logic for get world rate limit cache key.
        private static string GetWorldRateLimitCacheKey(int senderId)
            => $"chat:world:sender:{senderId}:last-sent";

        // Initialize or configure conversation using player profile id, recipient id, total count, and items; it loads conversation cache key, materializes the query results, and updates cache value.
        private async Task CacheConversation(
            int playerProfileId,
            int recipientId,
            int totalCount,
            List<ChatMessageResponseDto> items)
        {
            var cacheKey = GetConversationCacheKey(playerProfileId, recipientId);
            var cached = new CachedConversation
            {
                TotalCount = totalCount,
                Items = items.TakeLast(MaxCachedMessages).ToList()
            };

            await SetCacheValue(cacheKey, cached, ConversationCacheTtl);
        }

        // Process append to conversation cache using sender id, recipient id, and message; it loads conversation cache key, loads cache value, creates add, orders the resulting records, and materializes the query results and guards invalid or unavailable states.
        private async Task AppendToConversationCache(
            int senderId,
            int recipientId,
            ChatMessageResponseDto message)
        {
            var cacheKey = GetConversationCacheKey(senderId, recipientId);
            var cached = await GetCacheValue<CachedConversation>(cacheKey);
            if (cached == null)  // Entity not found — short-circuit with appropriate error result
                return;

            cached.Items.Add(message);
            cached.TotalCount += 1;
            cached.Items = cached.Items
                .OrderBy(x => x.SentAt)  // Sort results oldest/lowest first
                .ThenBy(x => x.ChatMessageId)
                .TakeLast(MaxCachedMessages)
                .ToList();

            await SetCacheValue(cacheKey, cached, ConversationCacheTtl);
        }

        // Executes core business logic for replace conversation cache message.
        // Logic details: validates required non-empty string arguments; validates numeric boundary constraints; checks Redis/memory cache to minimize database load.
        // Completes asynchronously upon successful execution.
        private async Task ReplaceConversationCacheMessage(ChatMessageResponseDto message)
        {
            var cacheKey = GetConversationCacheKey(message.SenderId, message.RecipientId);
            var cached = await GetCacheValue<CachedConversation>(cacheKey);
            if (cached == null)  // Entity not found — short-circuit with appropriate error result
                return;

            var index = cached.Items.FindIndex(x => x.ChatMessageId == message.ChatMessageId);
            if (index < 0)
                return;

            cached.Items[index] = message;
            await SetCacheValue(cacheKey, cached, ConversationCacheTtl);
        }
        // Process the supplied values: selects one of the two return values from the input condition and normalizes or validates the text before returning the derived result.
        private async Task<T?> GetCacheValue<T>(string key) where T : class
        {
            try
            {
                var json = await _cache.GetStringAsync(key);  // Look up precomputed value in distributed Redis cache
                return string.IsNullOrWhiteSpace(json)
                    ? null
                    : JsonSerializer.Deserialize<T>(json, CacheJsonOptions);
            }
            catch
            {
                return null;
            }
        }

        // Update cache value using key, value, and ttl; it updates string async.
        private async Task SetCacheValue<T>(string key, T value, TimeSpan ttl) where T : class
        {
            try
            {
                var json = JsonSerializer.Serialize(value, CacheJsonOptions);
                await _cache.SetStringAsync(  // Cache result with TTL to reduce repeated DB lookups
                    key,
                    json,
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = ttl
                    });
            }
            catch
            {
            }
        }

        // Executes core business logic for get cached date time.
        // Logic details: validates required non-empty string arguments; checks Redis/memory cache to minimize database load.
        // Returns the computed DateTime? result asynchronously.
        private async Task<DateTime?> GetCachedDateTime(string key)
        {
            try
            {
                var value = await _cache.GetStringAsync(key);  // Look up precomputed value in distributed Redis cache
                if (string.IsNullOrWhiteSpace(value))  // Mandatory string argument is blank — fail fast
                    return null;

                return DateTime.TryParse(value, null, DateTimeStyles.RoundtripKind, out var dateTime)
                    ? dateTime
                    : null;
            }
            catch
            {
                return null;
            }
        }

        // Executes core business logic for set cached date time.
        // Logic details: stores computed result in cache with an expiration TTL.
        // Completes asynchronously upon successful execution.
        private async Task SetCachedDateTime(string key, DateTime value, TimeSpan ttl)
        {
            try
            {
                await _cache.SetStringAsync(  // Cache result with TTL to reduce repeated DB lookups
                    key,
                    value.ToString("O", CultureInfo.InvariantCulture),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = ttl
                    });
            }
            catch
            {
            }
        }

        // Executes core business logic for get conversation cache key.
        private static string GetConversationCacheKey(int firstPlayerId, int secondPlayerId)
        {
            var min = Math.Min(firstPlayerId, secondPlayerId);
            var max = Math.Max(firstPlayerId, secondPlayerId);
            return $"chat:conversation:{min}:{max}:latest";
        }

        // Executes core business logic for cached conversation.
        private sealed class CachedConversation
        {
            // Executes core business logic for total count.
            public int TotalCount { get; set; }
            // Executes core business logic for items.
            public List<ChatMessageResponseDto> Items { get; set; } = new();
        }

        // Executes core business logic for cached world messages.
        private sealed class CachedWorldMessages
        {
            // Executes core business logic for total count.
            public int TotalCount { get; set; }
            // Executes core business logic for items.
            public List<WorldChatMessageResponseDto> Items { get; set; } = new();
        }
    }

    // Executes core business logic for exception.
    public class ChatRateLimitException : Exception
    {
        // Executes core business logic for retry after seconds.
        public int RetryAfterSeconds { get; }

        // Executes core business logic for chat rate limit exception.
        public ChatRateLimitException(int retryAfterSeconds)
            : base($"Please wait {retryAfterSeconds} seconds before sending another chat message.")
        {
            RetryAfterSeconds = retryAfterSeconds;
        }
    }
}
