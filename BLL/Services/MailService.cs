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
    public class MailService : IMailService
    {
        private readonly IMailRepository _repository;
        private readonly IPlayerProfileRepository _playerProfileRepository;
        private readonly IInventoryService _inventoryService;

        private readonly IMapper _mapper;

        public MailService(
            IMailRepository repository,
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
        // Lấy danh sách mail của player có phân trang.
        public async Task<MailListPagedDto> GetMyMails(int playerProfileId, int page, int pageSize)
        {
            var (totalCount, items) = await _repository.GetMailsByPlayerIdPaged(playerProfileId, page, pageSize);

            var summaries = _mapper.Map<List<MailSummaryDto>>(items);

            return new MailListPagedDto
            {
                TotalMails = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                Items = summaries
            };
        }

        // Lấy chi tiết 1 mail.
        public async Task<MailDetailDto?> GetMailById(int mailId)
        {
            var mail = await _repository.GetMailById(mailId);
            return mail == null ? null : _mapper.Map<MailDetailDto>(mail);
        }

        // Đánh dấu mail đã đọc.
        public async Task<MailDetailDto> MarkMailAsRead(int mailId)
        {
            var mail = await _repository.GetMailById(mailId)
                ?? throw new KeyNotFoundException($"Mail with id {mailId} not found.");

            if (!mail.IsRead)
            {
                mail.IsRead = true;
                await _repository.UpdateMail(mail);
            }

            return _mapper.Map<MailDetailDto>(mail);
        }

        // Nhận phần thưởng từ mail.
        public async Task<MailDetailDto> ClaimMailReward(int mailId)
        {
            var mail = await _repository.GetMailById(mailId)
                ?? throw new KeyNotFoundException($"Mail with id {mailId} not found.");

            if (mail.IsClaimed)
                throw new InvalidOperationException("Reward has already been claimed.");

            if (mail.ExpiredAt != null && mail.ExpiredAt < DateTime.UtcNow)
                throw new InvalidOperationException("Mail has expired.");

            var playerProfile = await _playerProfileRepository.GetByIdFull(mail.PlayerProfileId)
                ?? throw new KeyNotFoundException("Player profile not found.");

            if (mail.AttachedGold > 0)
                playerProfile.Gold += mail.AttachedGold;

            if (mail.AttachedGems > 0)
                playerProfile.Gems += mail.AttachedGems;

            // Xử lý items
            if (mail.AttachedItems != null && mail.AttachedItems.Any())
            {
                foreach (var rewardItem in mail.AttachedItems)
                {
                    await _inventoryService.AddItemToInventory(
                        mail.PlayerProfileId,
                        rewardItem.ItemId,
                        rewardItem.Quantity);
                }
            }

            playerProfile.UpdatedAt = DateTime.UtcNow;
            await _playerProfileRepository.UpdatePlayerProfile(playerProfile);

            mail.IsClaimed = true;
            mail.IsRead = true;
            var updated = await _repository.UpdateMail(mail);

            return _mapper.Map<MailDetailDto>(updated);
        }

        // Xóa mềm mail (chỉ mail của chính player đó).
        public async Task DeleteMail(int mailId, int playerProfileId)
        {
            var mail = await _repository.GetMailById(mailId)
                ?? throw new KeyNotFoundException($"Mail with id {mailId} not found.");

            if (mail.PlayerProfileId != playerProfileId)
                throw new UnauthorizedAccessException("You can only delete your own mail.");

            if (mail.IsDeleted)
                throw new InvalidOperationException("Mail has already been deleted.");

            await _repository.SoftDeleteMail(mailId);
        }

        // ─── Admin APIs ─────────────────────────────────────────────────────────

        // Admin: lấy tất cả mail có lọc và phân trang.
        public async Task<PagedResultDto<MailDetailDto>> GetMailsPaged(
            int page, int pageSize, string? search, bool? isRead, bool? isClaimed)
        {
            var (totalCount, items) = await _repository.GetMailsPaged(page, pageSize, search, isRead, isClaimed);
            var dtos = _mapper.Map<List<MailDetailDto>>(items);
            return new PagedResultDto<MailDetailDto>(totalCount, dtos);
        }

            // Admin: gửi mail đến danh sách player.
        public async Task SendMailByListId(SendMailByListIdDto request)
        {
            if (request.PlayerProfileIds == null || request.PlayerProfileIds.Count == 0)
                throw new ArgumentException("Player profile IDs cannot be empty.");

            var mails = request.PlayerProfileIds.Select(id => new Mail
            {
                PlayerProfileId = id,
                Title = request.Title,
                Content = request.Content,
                Type = request.Type,
                AttachedItems = request.AttachedItems?.Select(item => new MailRewardItem
                {
                    ItemId = item.ItemId,
                    Quantity = item.Quantity
                }).ToList() ?? new List<MailRewardItem>(),
                IsRead = false,
                IsClaimed = false,
                SentAt = DateTime.UtcNow,
                ExpiredAt = request.ExpiredAt
            }).ToList();

            await _repository.CreateBulkMails(mails);
        }

        // Admin: broadcast mail đến toàn bộ player.
        public async Task SendMailToAll(SendMailToAllDto request)
        {
            var players = await _playerProfileRepository.GetAllPlayerProfiles();
            if (!players.Any())
                throw new ArgumentException("No players found to send mail.");

            var mails = players.Select(player => new Mail
            {
                PlayerProfileId = player.PlayerProfileId,
                Title = request.Title,
                Content = request.Content,
                Type = request.Type,
                AttachedItems = request.AttachedItems?.Select(item => new MailRewardItem
                {
                    ItemId = item.ItemId,
                    Quantity = item.Quantity
                }).ToList() ?? new List<MailRewardItem>(),
                IsRead = false,
                IsClaimed = false,
                SentAt = DateTime.UtcNow,
                ExpiredAt = request.ExpiredAt
            }).ToList();

            await _repository.CreateBulkMails(mails);
        }

        // ─── Private Mappers ────────────────────────────────────────────────────


    }
}
