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

        public async Task<MailResponseDto?> GetMailById(int id)
        {
            var mail = await _repository.GetMailById(id);
            if (mail == null)
                return null;
            return MapToResponseDto(mail);
        }

        public async Task<List<MailResponseDto>> GetMailsByPlayerId(int playerProfileId)
        {
            var mails = await _repository.GetMailsByPlayerId(playerProfileId);
            return mails.ConvertAll(m => MapToResponseDto(m));
        }

        public async Task<PagedResultDto<MailResponseDto>> GetMailsByPlayerIdPaged(int playerProfileId, int page, int pageSize)
        {
            var (totalCount, items) = await _repository.GetMailsByPlayerIdPaged(playerProfileId, page, pageSize);
            var dtos = items.Select(m => MapToResponseDto(m)).ToList();
            return new PagedResultDto<MailResponseDto>(totalCount, dtos);
        }

        public async Task<PlayerMeMailsResponseDto> GetMeMails(int playerProfileId)
        {
            var mails = await _repository.GetMailsByPlayerId(playerProfileId);
            var profile = await _playerProfileRepository.GetPlayerProfileById(playerProfileId);
            var playerName = profile?.DisplayName ?? "";

            var dtos = mails.Select(m => new MailResponseDto
            {
                MailId = m.MailId,
                PlayerProfileId = m.PlayerProfileId,
                PlayerName = playerName,
                Title = m.Title,
                Content = m.Content,
                Type = m.Type,
                AttachedGold = m.AttachedGold,
                AttachedGems = m.AttachedGems,
                AttachedItemId = m.AttachedItemId,
                AttachedItemName = m.AttachedItem?.Name,
                AttachedItemQuantity = m.AttachedItemQuantity,
                IsRead = m.IsRead,
                IsClaimed = m.IsClaimed,
                IsDeleted = m.IsDeleted,
                DeletedAt = m.DeletedAt,
                SentAt = m.SentAt,
                ExpiredAt = m.ExpiredAt
            }).ToList();

            return new PlayerMeMailsResponseDto
            {
                PlayerProfileId = playerProfileId,
                Mails = dtos,
                TotalCount = dtos.Count,
                UnreadCount = dtos.Count(m => !m.IsRead && !m.IsDeleted)
            };
        }

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
                AttachedGold = request.AttachedGold,
                AttachedGems = request.AttachedGems,
                AttachedItemId = request.AttachedItemId,
                AttachedItemQuantity = request.AttachedItemQuantity,
                IsRead = false,
                IsClaimed = false,
                SentAt = DateTime.UtcNow,
                ExpiredAt = request.ExpiredAt
            }).ToList();

            await _repository.CreateBulkMails(mails);
        }

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
                AttachedGold = request.AttachedGold,
                AttachedGems = request.AttachedGems,
                AttachedItemId = request.AttachedItemId,
                AttachedItemQuantity = request.AttachedItemQuantity,
                IsRead = false,
                IsClaimed = false,
                SentAt = DateTime.UtcNow,
                ExpiredAt = request.ExpiredAt
            }).ToList();

            await _repository.CreateBulkMails(mails);
        }

        public async Task<MailResponseDto> MarkMailAsRead(int mailId)
        {
            var mail = await _repository.GetMailById(mailId)
                ?? throw new KeyNotFoundException($"Mail with id {mailId} not found.");

            mail.IsRead = true;
            var updated = await _repository.UpdateMail(mail);

            var playerProfile = await _playerProfileRepository.GetPlayerProfileById(mail.PlayerProfileId);
            return MapToResponseDto(updated, playerProfile?.DisplayName);
        }

        public async Task<MailResponseDto> ClaimMailReward(int mailId)
        {
            var mail = await _repository.GetMailById(mailId)
                ?? throw new KeyNotFoundException($"Mail with id {mailId} not found.");

            if (mail.IsClaimed)
                throw new InvalidOperationException("Reward has already been claimed.");

            if (mail.ExpiredAt != null && mail.ExpiredAt < DateTime.UtcNow)
                throw new InvalidOperationException("Mail has expired.");

            var playerProfile = await _playerProfileRepository.GetByIdFull(mail.PlayerProfileId)
                ?? throw new KeyNotFoundException($"Player profile not found.");

            if (mail.AttachedGold > 0)
            {
                playerProfile.Gold += mail.AttachedGold;
            }

            if (mail.AttachedGems > 0)
            {
                playerProfile.Gems += mail.AttachedGems;
            }

            if (mail.AttachedItemId.HasValue && mail.AttachedItemQuantity > 0)
            {
                await _inventoryService.AddItemToInventory(
                    mail.PlayerProfileId,
                    mail.AttachedItemId.Value,
                    mail.AttachedItemQuantity);
            }

            playerProfile.UpdatedAt = DateTime.UtcNow;
            await _playerProfileRepository.UpdatePlayerProfile(playerProfile);

            mail.IsClaimed = true;
            var updated = await _repository.UpdateMail(mail);

            return MapToResponseDto(updated, playerProfile.DisplayName);
        }

        public async Task<MailResponseDto> DeleteMail(int mailId, int playerProfileId)
        {
            var mail = await _repository.GetMailById(mailId)
                ?? throw new KeyNotFoundException($"Mail with id {mailId} not found.");

            if (mail.PlayerProfileId != playerProfileId)
                throw new UnauthorizedAccessException("You can only delete your own mail.");

            if (mail.IsDeleted)
                throw new InvalidOperationException("Mail has already been deleted.");

            var deleted = await _repository.SoftDeleteMail(mailId);
            return MapToResponseDto(deleted);
        }

        private static MailResponseDto MapToResponseDto(Mail mail, string? playerName = null)
        {
            return new MailResponseDto
            {
                MailId = mail.MailId,
                PlayerProfileId = mail.PlayerProfileId,
                PlayerName = playerName ?? mail.PlayerProfile?.DisplayName,
                Title = mail.Title,
                Content = mail.Content,
                Type = mail.Type,
                AttachedGold = mail.AttachedGold,
                AttachedGems = mail.AttachedGems,
                AttachedItemId = mail.AttachedItemId,
                AttachedItemName = mail.AttachedItem?.Name,
                AttachedItemQuantity = mail.AttachedItemQuantity,
                IsRead = mail.IsRead,
                IsClaimed = mail.IsClaimed,
                IsDeleted = mail.IsDeleted,
                DeletedAt = mail.DeletedAt,
                SentAt = mail.SentAt,
                ExpiredAt = mail.ExpiredAt
            };
        }

        public async Task<PagedResultDto<MailResponseDto>> GetMailsPaged(int page, int pageSize, string? search, bool? isRead, bool? isClaimed)
        {
            var (totalCount, items) = await _repository.GetMailsPaged(page, pageSize, search, isRead, isClaimed);

            var dtos = items.Select(m => new MailResponseDto
            {
                MailId = m.MailId,
                PlayerProfileId = m.PlayerProfileId,
                PlayerName = m.PlayerProfile?.DisplayName,
                Title = m.Title,
                Content = m.Content,
                Type = m.Type,
                AttachedGold = m.AttachedGold,
                AttachedGems = m.AttachedGems,
                AttachedItemId = m.AttachedItemId,
                AttachedItemName = m.AttachedItem?.Name,
                AttachedItemQuantity = m.AttachedItemQuantity,
                IsRead = m.IsRead,
                IsClaimed = m.IsClaimed,
                IsDeleted = m.IsDeleted,
                DeletedAt = m.DeletedAt,
                SentAt = m.SentAt,
                ExpiredAt = m.ExpiredAt
            }).ToList();

            return new PagedResultDto<MailResponseDto>(totalCount, dtos);
        }
    }
}
