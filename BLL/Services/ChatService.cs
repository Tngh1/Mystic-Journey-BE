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
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;

        public ChatService(
            IChatMessageRepository repository,
            IPlayerProfileRepository playerProfileRepository,
            IMapper mapper,
            IDistributedCache cache)
        {
            _repository = repository;
            _playerProfileRepository = playerProfileRepository;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<PagedResultDto<WorldChatMessageResponseDto>> GetWorldMessages(
            int playerProfileId,
            WorldChatMessageListQueryDto query)
        {
            if (playerProfileId <= 0)
                throw new UnauthorizedAccessException("Player profile not found.");

            var page = Math.Max(1, query.Page);
            var pageSize = Math.Clamp(query.PageSize, 1, MaxCachedWorldMessages);

            await EnsurePlayerExists(playerProfileId, "Player profile not found.");

            if (page == 1)
            {
                var cached = await GetCacheValue<CachedWorldMessages>(GetWorldCacheKey());
                if (cached != null)
                {
                    var cachedItems = cached.Items
                        .TakeLast(pageSize)
                        .ToList();

                    return new PagedResultDto<WorldChatMessageResponseDto>(cached.TotalCount, cachedItems);
                }
            }

            var repositoryPageSize = page == 1 ? MaxCachedWorldMessages : pageSize;
            var (totalCount, messages) = await _repository.GetWorldMessagesPaged(page, repositoryPageSize);

            var dtos = _mapper.Map<List<WorldChatMessageResponseDto>>(messages);
            dtos.Reverse();

            if (page == 1)
            {
                await CacheWorldMessages(totalCount, dtos);
                dtos = dtos.TakeLast(pageSize).ToList();
            }

            return new PagedResultDto<WorldChatMessageResponseDto>(totalCount, dtos);
        }

        public async Task<WorldChatMessageResponseDto> SendWorldMessage(
            int senderId,
            SendWorldChatMessageRequestDto request)
        {
            if (senderId <= 0)
                throw new UnauthorizedAccessException("Player profile not found.");

            var content = NormalizeContent(request.Content);
            await EnsurePlayerExists(senderId, "Player profile not found.");

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

                var message = _mapper.Map<WorldChatMessage>(request);
                message.SenderId = senderId;
                message.Content = content;
                message.IsReported = false;
                message.IsHidden = false;
                message.SentAt = now;

                var created = await _repository.CreateWorldMessage(message);
                var dto = _mapper.Map<WorldChatMessageResponseDto>(created);

                await SetCachedDateTime(rateLimitKey, dto.SentAt, WorldSendCooldown);
                await AppendToWorldCache(dto);

                return dto;
            }
            finally
            {
                senderLock.Release();
            }
        }

        public async Task<WorldChatMessageResponseDto> ReportWorldMessage(
            int reporterId,
            ReportChatMessageRequestDto request)
        {
            if (reporterId <= 0)
                throw new UnauthorizedAccessException("Player profile not found.");

            if (request.ChatMessageId <= 0)
                throw new BadRequestException("Chat message ID must be greater than 0.");

            var reason = NormalizeOptionalText(
                request.Reason,
                500,
                "Reason must not exceed 500 characters.");

            await EnsurePlayerExists(reporterId, "Player profile not found.");

            var message = await _repository.GetWorldMessageById(request.ChatMessageId);
            if (message == null)
                throw new KeyNotFoundException("World chat message not found.");

            message.IsReported = true;
            message.ReportedById = reporterId;
            message.ReportReason = reason;
            message.ReportedAt = DateTime.UtcNow;

            var updated = await _repository.UpdateWorldMessage(message);
            var dto = _mapper.Map<WorldChatMessageResponseDto>(updated);

            await ReplaceWorldCacheMessage(dto);

            return dto;
        }

        public async Task<PagedResultDto<ChatMessageResponseDto>> GetMessages(
            int playerProfileId,
            ChatMessageListQueryDto query)
        {
            if (playerProfileId <= 0)
                throw new UnauthorizedAccessException("Player profile not found.");

            var recipientId = query.RecipientId;
            if (recipientId <= 0)
                throw new BadRequestException("Recipient ID must be greater than 0.");

            if (playerProfileId == recipientId)
                throw new BadRequestException("You cannot open a chat with yourself.");

            var page = Math.Max(1, query.Page);
            var pageSize = Math.Clamp(query.PageSize, 1, MaxCachedMessages);

            await EnsurePlayerExists(playerProfileId, "Player profile not found.");
            await EnsurePlayerExists(recipientId, "Recipient profile not found.");

            if (page == 1 && pageSize <= MaxCachedMessages)
            {
                var cacheKey = GetConversationCacheKey(playerProfileId, recipientId);
                var cached = await GetCacheValue<CachedConversation>(cacheKey);
                if (cached != null)
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

            var dtos = _mapper.Map<List<ChatMessageResponseDto>>(messages);
            dtos.Reverse();

            if (page == 1)
            {
                await CacheConversation(playerProfileId, recipientId, totalCount, dtos);
                dtos = dtos.TakeLast(pageSize).ToList();
            }

            return new PagedResultDto<ChatMessageResponseDto>(totalCount, dtos);
        }

        public async Task<ChatMessageResponseDto> SendMessage(
            int senderId,
            SendChatMessageRequestDto request)
        {
            if (senderId <= 0)
                throw new UnauthorizedAccessException("Player profile not found.");

            if (request.RecipientId <= 0)
                throw new BadRequestException("Recipient ID must be greater than 0.");

            if (senderId == request.RecipientId)
                throw new BadRequestException("You cannot send a message to yourself.");

            var content = NormalizeContent(request.Content);

            await EnsurePlayerExists(senderId, "Player profile not found.");
            await EnsurePlayerExists(request.RecipientId, "Recipient profile not found.");

            var now = DateTime.UtcNow;
            var message = _mapper.Map<ChatMessage>(request);
            message.SenderId = senderId;
            message.RecipientId = request.RecipientId;
            message.Content = content;
            message.IsReported = false;
            message.IsHidden = false;
            message.SentAt = now;

            var created = await _repository.Create(message);
            var dto = _mapper.Map<ChatMessageResponseDto>(created);

            await AppendToConversationCache(senderId, request.RecipientId, dto);

            return dto;
        }

        private async Task EnsurePlayerExists(int playerProfileId, string message)
        {
            var player = await _playerProfileRepository.GetPlayerProfileById(playerProfileId);
            if (player == null)
                throw new KeyNotFoundException(message);
        }

        private static string NormalizeContent(string? content)
        {
            var normalized = content?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(normalized))
                throw new BadRequestException("Content is required.");

            if (normalized.Length > 500)
                throw new BadRequestException("Content must not exceed 500 characters.");

            return normalized;
        }

        private static string? NormalizeOptionalText(string? value, int maxLength, string errorMessage)
        {
            var normalized = value?.Trim();
            if (string.IsNullOrWhiteSpace(normalized))
                return null;

            if (normalized.Length > maxLength)
                throw new BadRequestException(errorMessage);

            return normalized;
        }

        private static void ThrowIfCooldownActive(DateTime lastSentAt, DateTime now, TimeSpan cooldown)
        {
            var remaining = cooldown - (now - lastSentAt);
            if (remaining > TimeSpan.Zero)
            {
                throw new ChatRateLimitException((int)Math.Ceiling(remaining.TotalSeconds));
            }
        }

        private async Task CacheWorldMessages(int totalCount, List<WorldChatMessageResponseDto> items)
        {
            var cached = new CachedWorldMessages
            {
                TotalCount = totalCount,
                Items = items.TakeLast(MaxCachedWorldMessages).ToList()
            };

            await SetCacheValue(GetWorldCacheKey(), cached, WorldCacheTtl);
        }

        private async Task AppendToWorldCache(WorldChatMessageResponseDto message)
        {
            var cacheKey = GetWorldCacheKey();
            var cached = await GetCacheValue<CachedWorldMessages>(cacheKey);
            if (cached == null)
            {
                await RefreshWorldCacheFromDatabase();
                return;
            }

            cached.Items.Add(message);
            cached.TotalCount += 1;
            cached.Items = cached.Items
                .OrderBy(x => x.SentAt)
                .ThenBy(x => x.ChatMessageId)
                .TakeLast(MaxCachedWorldMessages)
                .ToList();

            await SetCacheValue(cacheKey, cached, WorldCacheTtl);
        }

        private async Task ReplaceWorldCacheMessage(WorldChatMessageResponseDto message)
        {
            var cacheKey = GetWorldCacheKey();
            var cached = await GetCacheValue<CachedWorldMessages>(cacheKey);
            if (cached == null)
                return;

            var index = cached.Items.FindIndex(x => x.ChatMessageId == message.ChatMessageId);
            if (index < 0)
                return;

            cached.Items[index] = message;
            await SetCacheValue(cacheKey, cached, WorldCacheTtl);
        }

        private async Task RefreshWorldCacheFromDatabase()
        {
            var (totalCount, messages) = await _repository.GetWorldMessagesPaged(1, MaxCachedWorldMessages);
            var dtos = _mapper.Map<List<WorldChatMessageResponseDto>>(messages);
            dtos.Reverse();
            await CacheWorldMessages(totalCount, dtos);
        }

        private static string GetWorldCacheKey()
            => "chat:world:latest";

        private static string GetWorldRateLimitCacheKey(int senderId)
            => $"chat:world:sender:{senderId}:last-sent";

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

        private async Task AppendToConversationCache(
            int senderId,
            int recipientId,
            ChatMessageResponseDto message)
        {
            var cacheKey = GetConversationCacheKey(senderId, recipientId);
            var cached = await GetCacheValue<CachedConversation>(cacheKey);
            if (cached == null)
                return;

            cached.Items.Add(message);
            cached.TotalCount += 1;
            cached.Items = cached.Items
                .OrderBy(x => x.SentAt)
                .ThenBy(x => x.ChatMessageId)
                .TakeLast(MaxCachedMessages)
                .ToList();

            await SetCacheValue(cacheKey, cached, ConversationCacheTtl);
        }

        private async Task<T?> GetCacheValue<T>(string key) where T : class
        {
            try
            {
                var json = await _cache.GetStringAsync(key);
                return string.IsNullOrWhiteSpace(json)
                    ? null
                    : JsonSerializer.Deserialize<T>(json, CacheJsonOptions);
            }
            catch
            {
                return null;
            }
        }

        private async Task SetCacheValue<T>(string key, T value, TimeSpan ttl) where T : class
        {
            try
            {
                var json = JsonSerializer.Serialize(value, CacheJsonOptions);
                await _cache.SetStringAsync(
                    key,
                    json,
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = ttl
                    });
            }
            catch
            {
                // Redis is an optimization for chat history; DB remains the source of truth.
            }
        }

        private async Task<DateTime?> GetCachedDateTime(string key)
        {
            try
            {
                var value = await _cache.GetStringAsync(key);
                if (string.IsNullOrWhiteSpace(value))
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

        private async Task SetCachedDateTime(string key, DateTime value, TimeSpan ttl)
        {
            try
            {
                await _cache.SetStringAsync(
                    key,
                    value.ToString("O", CultureInfo.InvariantCulture),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = ttl
                    });
            }
            catch
            {
                // Redis cache failure must not block a message already persisted to PostgreSQL.
            }
        }

        private static string GetConversationCacheKey(int firstPlayerId, int secondPlayerId)
        {
            var min = Math.Min(firstPlayerId, secondPlayerId);
            var max = Math.Max(firstPlayerId, secondPlayerId);
            return $"chat:conversation:{min}:{max}:latest";
        }

        private sealed class CachedConversation
        {
            public int TotalCount { get; set; }
            public List<ChatMessageResponseDto> Items { get; set; } = new();
        }

        private sealed class CachedWorldMessages
        {
            public int TotalCount { get; set; }
            public List<WorldChatMessageResponseDto> Items { get; set; } = new();
        }
    }

    public class ChatRateLimitException : Exception
    {
        public int RetryAfterSeconds { get; }

        public ChatRateLimitException(int retryAfterSeconds)
            : base($"Please wait {retryAfterSeconds} seconds before sending another chat message.")
        {
            RetryAfterSeconds = retryAfterSeconds;
        }
    }
}
