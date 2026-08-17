using AutoMapper;
using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    // Executes core business logic for i mailbox service.
    public class MailboxService : IMailboxService
    {
        private const int MAILBOX_CAPACITY = 100;
        private readonly IMailboxRepository _repository;
        private readonly IPlayerProfileRepository _playerProfileRepository;
        private readonly IInventoryService _inventoryService;
        private readonly ITransactionManager _transactionManager;

        private readonly IMapper _mapper;

        // Initialize this instance from repository, player profile repository, inventory service, and mapper and store repository, player profile repository, inventory service, transaction manager, and mapper for later operations.
        public MailboxService(
            IMailboxRepository repository,
            IPlayerProfileRepository playerProfileRepository,
            IInventoryService inventoryService,
            IMapper mapper,
            ITransactionManager transactionManager)
        {
            _repository = repository;
            _playerProfileRepository = playerProfileRepository;
            _inventoryService = inventoryService;
            _transactionManager = transactionManager;
            _mapper = mapper;
        }

        // Executes core business logic for get my mailboxes.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models.
        // Returns the computed MailboxListPagedDto result asynchronously.
        public async Task<MailboxListPagedDto> GetMyMailboxes(int playerProfileId, int page, int pageSize)
        {
            var (totalCount, items) = await _repository.GetMailboxesByPlayerIdPaged(playerProfileId, page, pageSize);

            var summaries = _mapper.Map<List<MailboxSummaryDto>>(items);  // Transform domain entity into DTO for the API response layer

            return new MailboxListPagedDto
            {
                TotalMailboxes = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                Items = summaries
            };
        }

        // Executes core business logic for get mailbox by id.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models; throws KeyNotFoundException on invalid state or rule violations.
        // Returns the computed MailboxDetailDto? result asynchronously.
        public async Task<MailboxDetailDto?> GetMailboxById(int mailboxId)
        {
            var mailbox = await _repository.GetMailboxById(mailboxId);
            return mailbox == null ? null : _mapper.Map<MailboxDetailDto>(mailbox);  // Transform domain entity into DTO for the API response layer
        }

        // Executes core business logic for mark mailbox as read.
        // Logic details: delegates data queries and updates to repository layer; throws KeyNotFoundException on invalid state or rule violations.
        // Returns the computed MailboxDetailDto result asynchronously.
        public async Task<MailboxDetailDto> MarkMailboxAsRead(int mailboxId)
        {
            var mailbox = await _repository.GetMailboxById(mailboxId)
                ?? throw new KeyNotFoundException($"Mailbox with id {mailboxId} not found.");

            if (!mailbox.IsRead)
            {
                mailbox.IsRead = true;
                await _repository.UpdateMailbox(mailbox);
            }

            return _mapper.Map<MailboxDetailDto>(mailbox);  // Transform domain entity into DTO for the API response layer
        }

        // Executes core business logic for claim mailbox reward.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models; throws InvalidOperationException, KeyNotFoundException on invalid state or rule violations.
        // Returns the computed MailboxDetailDto result asynchronously.
        public Task<MailboxDetailDto> ClaimMailboxReward(int mailboxId)
        {
            return _transactionManager.ExecuteInTransactionAsync(async () =>
            {
            var mailbox = await _repository.GetMailboxById(mailboxId)
                ?? throw new KeyNotFoundException($"Mailbox with id {mailboxId} not found.");

            if (mailbox.IsClaimed)
                throw new InvalidOperationException("Reward has already been claimed.");  // Unexpected runtime state — propagate to global error handler

            if (mailbox.ExpiredAt != null && mailbox.ExpiredAt < DateTime.UtcNow)
                throw new InvalidOperationException("Mailbox has expired.");  // Unexpected runtime state — propagate to global error handler

            var playerProfile = await _playerProfileRepository.GetByIdFull(mailbox.PlayerProfileId)
                ?? throw new KeyNotFoundException("Player profile not found.");

            if (mailbox.AttachedGold > 0)
                playerProfile.Gold += mailbox.AttachedGold;

            if (mailbox.AttachedGems > 0)
                playerProfile.Gems += mailbox.AttachedGems;

            if (mailbox.AttachedItems != null && mailbox.AttachedItems.Any())
            {
                foreach (var rewardItem in mailbox.AttachedItems)
                {
                    if (string.Equals(rewardItem.Item?.Name, "Exp", StringComparison.OrdinalIgnoreCase))
                    {
                        playerProfile.AddExperience(rewardItem.Quantity);
                        continue;
                    }

                    await _inventoryService.AddItemToInventory(
                        mailbox.PlayerProfileId,
                        rewardItem.ItemId,
                        rewardItem.Quantity);
                }
            }

            playerProfile.UpdatedAt = DateTime.UtcNow;
            await _playerProfileRepository.UpdatePlayerProfile(playerProfile);

            mailbox.IsClaimed = true;
            mailbox.IsRead = true;
            var updated = await _repository.UpdateMailbox(mailbox);

                return _mapper.Map<MailboxDetailDto>(updated);  // Transform domain entity into DTO for the API response layer
            });
        }

        // Executes core business logic for delete mailbox.
        // Logic details: delegates data queries and updates to repository layer; throws KeyNotFoundException on invalid state or rule violations.
        // Completes asynchronously upon successful execution.
        public async Task DeleteMailbox(int mailboxId, int playerProfileId)
        {
            var mailbox = await _repository.GetMailboxById(mailboxId)
                ?? throw new KeyNotFoundException($"Mailbox with id {mailboxId} not found.");

            if (mailbox.PlayerProfileId != playerProfileId)
                throw new UnauthorizedAccessException("You can only delete your own mailbox.");  // Authentication token is invalid or expired

            if (mailbox.IsDeleted)
                throw new InvalidOperationException("Mailbox has already been deleted.");  // Unexpected runtime state — propagate to global error handler

            if (HasUnclaimedAttachment(mailbox))
                throw new InvalidOperationException(  // Unexpected runtime state — propagate to global error handler
                    "This mail still has unclaimed rewards. Please claim the rewards before deleting it.");

            await _repository.SoftDeleteMailbox(mailboxId);
        }

        // Executes core business logic for has unclaimed attachment.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models.
        // Returns a boolean indicating operation success.
        private static bool HasUnclaimedAttachment(Mailbox mailbox)
        {
            if (mailbox.IsClaimed)
                return false;

            return mailbox.AttachedGold > 0
                || mailbox.AttachedGems > 0
                || (mailbox.AttachedItems != null && mailbox.AttachedItems.Any(i => i.Quantity > 0));
        }

        // Executes core business logic for is expired.
        // Logic details: delegates data queries and updates to repository layer; transforms domain entities into DTO transfer models.
        // Returns a boolean indicating operation success.
        private static bool IsExpired(Mailbox mailbox)
            => mailbox.ExpiredAt != null && mailbox.ExpiredAt < DateTime.UtcNow;


        // Load mailboxes paged using page, page size, search, and is read; it builds map.
        public async Task<PagedResultDto<MailboxDetailDto>> GetMailboxesPaged(
            int page, int pageSize, string? search, bool? isRead, bool? isClaimed, string? sortBy = null, string? sortOrder = null)
        {
            var (totalCount, items) = await _repository.GetMailboxesPaged(page, pageSize, search, isRead, isClaimed, sortBy, sortOrder);
            var dtos = _mapper.Map<List<MailboxDetailDto>>(items);  // Transform domain entity into DTO for the API response layer
            return new PagedResultDto<MailboxDetailDto>(totalCount, dtos);
        }

        // Executes core business logic for send mailbox by list id.
        // Logic details: validates numeric boundary constraints; throws ArgumentException on invalid state or rule violations.
        // Returns the computed List<MailboxDetailDto result asynchronously.
        public async Task<List<MailboxDetailDto>> SendMailboxByListId(SendMailboxByListIdDto request)
        {
            if (request.PlayerProfileIds == null || request.PlayerProfileIds.Count == 0)
                throw new ArgumentException("Player profile IDs cannot be empty.");

            if (request.AttachedGold < 0 || request.AttachedGold > 9999)
                throw new ArgumentException("Attached gold must be between 0 and 9999.");

            if (request.AttachedGems < 0 || request.AttachedGems > 9999)
                throw new ArgumentException("Attached gems must be between 0 and 9999.");

            if (request.AttachedItems != null && request.AttachedItems.Any(i => i != null && (i.Quantity < 1 || i.Quantity > 99)))
                throw new ArgumentException("Item quantity must be between 1 and 99.");

            var mailboxes = request.PlayerProfileIds.Select(id => new Mailbox
            {
                PlayerProfileId = id,
                Title = request.Title,
                Content = request.Content,
                Type = request.Type,
                AttachedGold = request.AttachedGold,
                AttachedGems = request.AttachedGems,
                AttachedItems = (request.AttachedItems ?? new List<SendMailboxRewardItemDto>())
                    .Where(i => i != null && i.ItemId > 0 && i.Quantity > 0)  // Filter records matching the predicate
                    .Select(item => new MailboxRewardItem
                    {
                        ItemId = item.ItemId,
                        Quantity = Math.Min(item.Quantity, 99)
                    }).ToList(),
                IsRead = false,
                IsClaimed = false,
                SentAt = DateTime.UtcNow,
                ExpiredAt = request.ExpiredAt
            }).ToList();

            return await _transactionManager.ExecuteInTransactionAsync(async () =>
            {
                foreach (var playerId in request.PlayerProfileIds.Distinct())
                    await EnsureMailboxCapacity(playerId);
                var createdMailboxes = await _repository.CreateBulkMailboxes(mailboxes);
                return _mapper.Map<List<MailboxDetailDto>>(createdMailboxes);  // Transform domain entity into DTO for the API response layer
            }, System.Data.IsolationLevel.Serializable);
        }

        // Executes core business logic for send mailbox to all.
        // Logic details: validates numeric boundary constraints; delegates data queries and updates to repository layer; throws ArgumentException on invalid state or rule violations.
        // Returns the computed List<MailboxDetailDto result asynchronously.
        public async Task<List<MailboxDetailDto>> SendMailboxToAll(SendMailboxToAllDto request)
        {
            var players = await _playerProfileRepository.GetAllPlayerProfiles();
            if (!players.Any())
                throw new ArgumentException("No players found to send mailbox.");

            if (request.AttachedGold < 0 || request.AttachedGold > 9999)
                throw new ArgumentException("Attached gold must be between 0 and 9999.");

            if (request.AttachedGems < 0 || request.AttachedGems > 9999)
                throw new ArgumentException("Attached gems must be between 0 and 9999.");

            if (request.AttachedItems != null && request.AttachedItems.Any(i => i != null && (i.Quantity < 1 || i.Quantity > 99)))
                throw new ArgumentException("Item quantity must be between 1 and 99.");

            var mailboxes = players.Select(player => new Mailbox
            {
                PlayerProfileId = player.PlayerProfileId,
                Title = request.Title,
                Content = request.Content,
                Type = request.Type,
                AttachedGold = request.AttachedGold,
                AttachedGems = request.AttachedGems,
                AttachedItems = (request.AttachedItems ?? new List<SendMailboxRewardItemDto>())
                    .Where(i => i != null && i.ItemId > 0 && i.Quantity > 0)  // Filter records matching the predicate
                    .Select(item => new MailboxRewardItem
                    {
                        ItemId = item.ItemId,
                        Quantity = Math.Min(item.Quantity, 99)
                    }).ToList(),
                IsRead = false,
                IsClaimed = false,
                SentAt = DateTime.UtcNow,
                ExpiredAt = request.ExpiredAt
            }).ToList();

            return await _transactionManager.ExecuteInTransactionAsync(async () =>
            {
                foreach (var player in players)
                    await EnsureMailboxCapacity(player.PlayerProfileId);
                var createdMailboxes = await _repository.CreateBulkMailboxes(mailboxes);
                return _mapper.Map<List<MailboxDetailDto>>(createdMailboxes);  // Transform domain entity into DTO for the API response layer
            }, System.Data.IsolationLevel.Serializable);
        }

        // Executes core business logic for ensure mailbox capacity.
        // Logic details: delegates data queries and updates to repository layer; throws InvalidOperationException on invalid state or rule violations.
        // Completes asynchronously upon successful execution.
        private async Task EnsureMailboxCapacity(int playerProfileId)
        {
            var activeMailboxes = await _repository.GetMailboxesByPlayerId(playerProfileId);
            while (activeMailboxes.Count >= MAILBOX_CAPACITY)
            {
                var removable = activeMailboxes
                    .Where(m => m.IsRead && !HasUnclaimedAttachment(m))  // Filter records matching the predicate
                    .OrderBy(m => m.SentAt)  // Sort results oldest/lowest first
                    .FirstOrDefault();

                if (removable == null)  // Entity not found — short-circuit with appropriate error result
                    throw new InvalidOperationException(  // Unexpected runtime state — propagate to global error handler
                        $"Mailbox capacity reached for player {playerProfileId}; no read mail without rewards can be removed.");

                await _repository.SoftDeleteMailbox(removable.MailboxId);
                activeMailboxes.Remove(removable);  // Mark entity for deletion in the next SaveChanges call
            }
        }
    }
}
