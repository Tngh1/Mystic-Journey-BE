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
        private readonly IMailRepository _mailRepository;
        private readonly IPlayerProfileRepository _profileRepository;
        private readonly IInventoryService _inventoryService;
        private readonly IPlayerProfileService _playerProfileService;

        public MailService(
            IMailRepository mailRepository,
            IPlayerProfileRepository profileRepository,
            IInventoryService inventoryService,
            IPlayerProfileService playerProfileService)
        {
            _mailRepository = mailRepository;
            _profileRepository = profileRepository;
            _inventoryService = inventoryService;
            _playerProfileService = playerProfileService;
        }

        public async Task<MailListResponseDto> GetMailsAsync(Guid accountId, int pageNumber = 1, int pageSize = 20)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new MailListResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var mails = await _mailRepository.GetByPlayerProfileIdAsync(profile.Id, pageNumber, pageSize);
            var totalCount = await _mailRepository.GetTotalCountAsync(profile.Id);
            var unreadCount = await _mailRepository.GetUnreadCountAsync(profile.Id);

            var dtos = mails.Select(m => new MailResponseDto
            {
                MailId = m.Id,
                PlayerProfileId = m.PlayerProfileId,
                Title = m.Title,
                Content = m.Content,
                Type = m.Type.ToString(),
                AttachedGold = m.AttachedGold,
                AttachedGems = m.AttachedGems,
                AttachedItemId = m.AttachedItemId,
                AttachedItemName = m.AttachedItem?.Name,
                AttachedItemQuantity = m.AttachedItemQuantity,
                IsRead = m.IsRead,
                IsClaimed = m.IsClaimed,
                SentAt = m.SentAt,
                ExpiredAt = m.ExpiredAt
            }).ToList();

            return new MailListResponseDto
            {
                Success = true,
                Message = "Mails retrieved successfully.",
                Mails = dtos,
                TotalCount = totalCount,
                UnreadCount = unreadCount
            };
        }

        public async Task<MailListResponseDto> GetUnreadMailsAsync(Guid accountId)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new MailListResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var mails = await _mailRepository.GetUnreadMailsAsync(profile.Id);

            var dtos = mails.Select(m => new MailResponseDto
            {
                MailId = m.Id,
                PlayerProfileId = m.PlayerProfileId,
                Title = m.Title,
                Content = m.Content,
                Type = m.Type.ToString(),
                AttachedGold = m.AttachedGold,
                AttachedGems = m.AttachedGems,
                AttachedItemId = m.AttachedItemId,
                AttachedItemName = m.AttachedItem?.Name,
                AttachedItemQuantity = m.AttachedItemQuantity,
                IsRead = m.IsRead,
                IsClaimed = m.IsClaimed,
                SentAt = m.SentAt,
                ExpiredAt = m.ExpiredAt
            }).ToList();

            return new MailListResponseDto
            {
                Success = true,
                Message = "Unread mails retrieved successfully.",
                Mails = dtos,
                TotalCount = dtos.Count,
                UnreadCount = dtos.Count
            };
        }

        public async Task<MailApiResponseDto> GetMailByIdAsync(Guid accountId, Guid mailId)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new MailApiResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var mail = await _mailRepository.GetByIdWithDetailsAsync(mailId);
            if (mail == null || mail.PlayerProfileId != profile.Id)
            {
                return new MailApiResponseDto
                {
                    Success = false,
                    Message = "Mail not found."
                };
            }

            if (!mail.IsRead)
            {
                mail.IsRead = true;
                await _mailRepository.UpdateAsync(mail);
            }

            var dto = new MailResponseDto
            {
                MailId = mail.Id,
                PlayerProfileId = mail.PlayerProfileId,
                Title = mail.Title,
                Content = mail.Content,
                Type = mail.Type.ToString(),
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

            return new MailApiResponseDto
            {
                Success = true,
                Message = "Mail retrieved successfully.",
                Mail = dto
            };
        }

        public async Task<MailApiResponseDto> MarkAsReadAsync(Guid accountId, Guid mailId)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new MailApiResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var mail = await _mailRepository.GetByIdAsync(mailId);
            if (mail == null || mail.PlayerProfileId != profile.Id)
            {
                return new MailApiResponseDto
                {
                    Success = false,
                    Message = "Mail not found."
                };
            }

            if (!mail.IsRead)
            {
                mail.IsRead = true;
                await _mailRepository.UpdateAsync(mail);
            }

            return new MailApiResponseDto
            {
                Success = true,
                Message = "Mail marked as read."
            };
        }

        public async Task<MailApiResponseDto> ClaimMailAsync(Guid accountId, Guid mailId)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new MailApiResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var mail = await _mailRepository.GetByIdWithDetailsAsync(mailId);
            if (mail == null || mail.PlayerProfileId != profile.Id)
            {
                return new MailApiResponseDto
                {
                    Success = false,
                    Message = "Mail not found."
                };
            }

            if (mail.IsClaimed)
            {
                return new MailApiResponseDto
                {
                    Success = false,
                    Message = "Mail rewards have already been claimed."
                };
            }

            if (mail.ExpiredAt.HasValue && mail.ExpiredAt < DateTime.UtcNow)
            {
                return new MailApiResponseDto
                {
                    Success = false,
                    Message = "This mail has expired."
                };
            }

            if (mail.AttachedGold > 0 || mail.AttachedGems > 0)
            {
                await _playerProfileService.AddCurrencyAsync(accountId, new CurrencyUpdateDto
                {
                    CurrencyType = (int)PlayerCurrencyLog.CurrencyType.Gold,
                    Amount = mail.AttachedGold
                });

                await _playerProfileService.AddCurrencyAsync(accountId, new CurrencyUpdateDto
                {
                    CurrencyType = (int)PlayerCurrencyLog.CurrencyType.Gems,
                    Amount = mail.AttachedGems
                });
            }

            if (mail.AttachedItemId.HasValue && mail.AttachedItemQuantity > 0)
            {
                await _inventoryService.AddItemToInventoryAsync(accountId, new AddItemToInventoryRequestDto
                {
                    ItemId = mail.AttachedItemId.Value,
                    Quantity = mail.AttachedItemQuantity
                });
            }

            mail.IsClaimed = true;
            await _mailRepository.UpdateAsync(mail);

            return new MailApiResponseDto
            {
                Success = true,
                Message = "Mail rewards claimed successfully!"
            };
        }

        public async Task<MailApiResponseDto> SendMailAsync(Guid accountId, SendMailRequestDto request)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new MailApiResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var receiver = await _profileRepository.GetByIdAsync(request.ReceiverId);
            if (receiver == null)
            {
                return new MailApiResponseDto
                {
                    Success = false,
                    Message = "Receiver not found."
                };
            }

            var mailType = Enum.IsDefined(typeof(Mail.MailType), request.MailType)
                ? (Mail.MailType)request.MailType
                : Mail.MailType.System;

            var mail = new Mail
            {
                Id = Guid.NewGuid(),
                PlayerProfileId = receiver.Id,
                Title = request.Title,
                Content = request.Content,
                Type = mailType,
                AttachedGold = request.AttachedGold ?? 0,
                AttachedGems = request.AttachedGems ?? 0,
                AttachedItemId = request.AttachedItemId,
                AttachedItemQuantity = request.AttachedItemQuantity,
                IsRead = false,
                IsClaimed = false,
                SentAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddDays(30)
            };

            await _mailRepository.CreateAsync(mail);

            return new MailApiResponseDto
            {
                Success = true,
                Message = $"Mail sent to {receiver.DisplayName} successfully!"
            };
        }

        public async Task<MailApiResponseDto> DeleteMailAsync(Guid accountId, Guid mailId)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new MailApiResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var mail = await _mailRepository.GetByIdAsync(mailId);
            if (mail == null || mail.PlayerProfileId != profile.Id)
            {
                return new MailApiResponseDto
                {
                    Success = false,
                    Message = "Mail not found."
                };
            }

            if (!mail.IsClaimed && (mail.AttachedGold > 0 || mail.AttachedGems > 0 || mail.AttachedItemId.HasValue))
            {
                return new MailApiResponseDto
                {
                    Success = false,
                    Message = "Please claim the rewards before deleting the mail."
                };
            }

            await _mailRepository.UpdateAsync(mail);

            return new MailApiResponseDto
            {
                Success = true,
                Message = "Mail deleted successfully."
            };
        }

        public async Task<int> GetUnreadCountAsync(Guid accountId)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null) return 0;

            return await _mailRepository.GetUnreadCountAsync(profile.Id);
        }
    }
}
