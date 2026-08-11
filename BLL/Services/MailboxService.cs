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
    public class MailboxService : IMailboxService
    {
        private readonly IMailboxRepository _repository;
        private readonly IPlayerProfileRepository _playerProfileRepository;
        private readonly IInventoryService _inventoryService;

        private readonly IMapper _mapper;

        public MailboxService(
            IMailboxRepository repository,
            IPlayerProfileRepository playerProfileRepository,
            IInventoryService inventoryService,
            IMapper mapper)
        {
            _repository = repository;
            _playerProfileRepository = playerProfileRepository;
            _inventoryService = inventoryService;
            _mapper = mapper;
        }

        // ─── Player APIs ────────────────────────────────────────────────────────
        // Lấy danh sách thư của player có phân trang.
        public async Task<MailboxListPagedDto> GetMyMailboxes(int playerProfileId, int page, int pageSize)
        {
            var (totalCount, items) = await _repository.GetMailboxesByPlayerIdPaged(playerProfileId, page, pageSize);

            var summaries = _mapper.Map<List<MailboxSummaryDto>>(items);

            return new MailboxListPagedDto
            {
                TotalMailboxes = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                Items = summaries
            };
        }

        // Lấy chi tiết 1 thư.
        public async Task<MailboxDetailDto?> GetMailboxById(int mailboxId)
        {
            var mailbox = await _repository.GetMailboxById(mailboxId);
            return mailbox == null ? null : _mapper.Map<MailboxDetailDto>(mailbox);
        }

        // Đánh dấu thư đã đọc.
        public async Task<MailboxDetailDto> MarkMailboxAsRead(int mailboxId)
        {
            var mailbox = await _repository.GetMailboxById(mailboxId)
                ?? throw new KeyNotFoundException($"Mailbox with id {mailboxId} not found.");

            if (!mailbox.IsRead)
            {
                mailbox.IsRead = true;
                await _repository.UpdateMailbox(mailbox);
            }

            return _mapper.Map<MailboxDetailDto>(mailbox);
        }

        // Nhận phần thưởng từ thư.
        public async Task<MailboxDetailDto> ClaimMailboxReward(int mailboxId)
        {
            var mailbox = await _repository.GetMailboxById(mailboxId)
                ?? throw new KeyNotFoundException($"Mailbox with id {mailboxId} not found.");

            if (mailbox.IsClaimed)
                throw new InvalidOperationException("Reward has already been claimed.");

            if (mailbox.ExpiredAt != null && mailbox.ExpiredAt < DateTime.UtcNow)
                throw new InvalidOperationException("Mailbox has expired.");

            var playerProfile = await _playerProfileRepository.GetByIdFull(mailbox.PlayerProfileId)
                ?? throw new KeyNotFoundException("Player profile not found.");

            if (mailbox.AttachedGold > 0)
                playerProfile.Gold += mailbox.AttachedGold;

            if (mailbox.AttachedGems > 0)
                playerProfile.Gems += mailbox.AttachedGems;

            // Xử lý items
            if (mailbox.AttachedItems != null && mailbox.AttachedItems.Any())
            {
                foreach (var rewardItem in mailbox.AttachedItems)
                {
                    // "Exp" là item Currency đại diện cho điểm kinh nghiệm -> cộng thẳng vào
                    // level thay vì bỏ vào inventory (khớp cách quest/dungeon áp EXP).
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

            return _mapper.Map<MailboxDetailDto>(updated);
        }

        // Xóa mềm thư (chỉ thư của chính player đó).
        public async Task DeleteMailbox(int mailboxId, int playerProfileId)
        {
            var mailbox = await _repository.GetMailboxById(mailboxId)
                ?? throw new KeyNotFoundException($"Mailbox with id {mailboxId} not found.");

            if (mailbox.PlayerProfileId != playerProfileId)
                throw new UnauthorizedAccessException("You can only delete your own mailbox.");

            if (mailbox.IsDeleted)
                throw new InvalidOperationException("Mailbox has already been deleted.");

            // BR-147: thư còn phần thưởng chưa nhận thì không được xoá, nếu không
            // người chơi xoá thư là mất luôn phần thưởng.
            // Thư đã hết hạn vẫn cho xoá: lúc đó reward không claim được nữa
            // (ClaimMailboxReward chặn ExpiredAt), nên giữ lại chỉ làm rác hộp thư.
            if (HasUnclaimedAttachment(mailbox) && !IsExpired(mailbox))
                throw new InvalidOperationException(
                    "This mail still has unclaimed rewards. Please claim the rewards before deleting it.");

            await _repository.SoftDeleteMailbox(mailboxId);
        }

        // Thư có đính kèm thật (gold / gems / item) mà chưa nhận.
        // Thư thông báo thuần (không đính kèm gì) vẫn xoá được bình thường.
        private static bool HasUnclaimedAttachment(Mailbox mailbox)
        {
            if (mailbox.IsClaimed)
                return false;

            return mailbox.AttachedGold > 0
                || mailbox.AttachedGems > 0
                || (mailbox.AttachedItems != null && mailbox.AttachedItems.Any(i => i.Quantity > 0));
        }

        private static bool IsExpired(Mailbox mailbox)
            => mailbox.ExpiredAt != null && mailbox.ExpiredAt < DateTime.UtcNow;

        // ─── Admin APIs ─────────────────────────────────────────────────────────

        // Admin: lấy tất cả thư có lọc và phân trang.
        public async Task<PagedResultDto<MailboxDetailDto>> GetMailboxesPaged(
            int page, int pageSize, string? search, bool? isRead, bool? isClaimed, string? sortBy = null, string? sortOrder = null)
        {
            var (totalCount, items) = await _repository.GetMailboxesPaged(page, pageSize, search, isRead, isClaimed, sortBy, sortOrder);
            var dtos = _mapper.Map<List<MailboxDetailDto>>(items);
            return new PagedResultDto<MailboxDetailDto>(totalCount, dtos);
        }

            // Admin: gửi thư đến danh sách player.
        public async Task SendMailboxByListId(SendMailboxByListIdDto request)
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
                    .Where(i => i != null && i.ItemId > 0 && i.Quantity > 0)
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

            await _repository.CreateBulkMailboxes(mailboxes);
        }

        // Admin: broadcast thư đến toàn bộ player.
        public async Task SendMailboxToAll(SendMailboxToAllDto request)
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
                    .Where(i => i != null && i.ItemId > 0 && i.Quantity > 0)
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

            await _repository.CreateBulkMailboxes(mailboxes);
        }
    }
}
