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
        private readonly IMapper _mapper;

        public MailService(
            IMailRepository repository,
            IPlayerProfileRepository playerProfileRepository,
            IMapper mapper)
        {
            _repository = repository;
            _playerProfileRepository = playerProfileRepository;
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

        public async Task<MailResponseDto> SendMail(SendMailRequestDto request)
        {
            var playerProfile = await _playerProfileRepository.GetPlayerProfileById(request.PlayerProfileId)
                ?? throw new KeyNotFoundException($"Player profile with id {request.PlayerProfileId} not found.");

            var mail = new Mail
            {
                PlayerProfileId = request.PlayerProfileId,
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
            };

            var created = await _repository.CreateMail(mail);
            return MapToResponseDto(created, playerProfile.DisplayName);
        }

        public async Task SendBulkMail(BulkSendMailRequestDto request)
        {
            if (request.PlayerProfileIds == null || !request.PlayerProfileIds.Any())
                throw new ArgumentException("Player profile IDs cannot be empty.");

            var mails = request.PlayerProfileIds.Select(playerProfileId => new Mail
            {
                PlayerProfileId = playerProfileId,
                Title = request.Title,
                Content = request.Content,
                Type = request.Type,
                AttachedGold = request.AttachedGold,
                AttachedGems = request.AttachedGems,
                IsRead = false,
                IsClaimed = false,
                SentAt = DateTime.UtcNow,
                ExpiredAt = null
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

            playerProfile.UpdatedAt = DateTime.UtcNow;
            await _playerProfileRepository.UpdatePlayerProfile(playerProfile);

            mail.IsClaimed = true;
            var updated = await _repository.UpdateMail(mail);

            return MapToResponseDto(updated, playerProfile.DisplayName);
        }

        private static MailResponseDto MapToResponseDto(Mail mail, string? playerName = null)
        {
            return new MailResponseDto
            {
                Id = mail.MailId,
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
                SentAt = mail.SentAt,
                ExpiredAt = mail.ExpiredAt
            };
        }

        public IQueryable<MailResponseDto> GetMailsQueryable()
        {
            return _repository.GetMailsQueryable()
                .Select(m => new MailResponseDto
                {
                    Id = m.MailId,
                    PlayerProfileId = m.PlayerProfileId,
                    PlayerName = m.PlayerProfile == null ? null : m.PlayerProfile.DisplayName,
                    Title = m.Title,
                    Content = m.Content,
                    Type = m.Type,
                    AttachedGold = m.AttachedGold,
                    AttachedGems = m.AttachedGems,
                    AttachedItemId = m.AttachedItemId,
                    AttachedItemName = m.AttachedItem == null ? null : m.AttachedItem.Name,
                    AttachedItemQuantity = m.AttachedItemQuantity,
                    IsRead = m.IsRead,
                    IsClaimed = m.IsClaimed,
                    SentAt = m.SentAt,
                    ExpiredAt = m.ExpiredAt
                })
                .AsQueryable();
        }
    }
}
