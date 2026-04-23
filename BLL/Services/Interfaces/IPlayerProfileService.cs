using BLL.DTOs;
using System;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IPlayerProfileService
    {
        Task<PlayerProfileApiResponseDto> CreateProfileAsync(Guid accountId, CreatePlayerProfileRequestDto request);
        Task<PlayerProfileApiResponseDto> GetProfileByAccountIdAsync(Guid accountId);
        Task<PlayerProfileApiResponseDto> GetProfileDetailByAccountIdAsync(Guid accountId);
        Task<PlayerProfileApiResponseDto> UpdateProfileAsync(Guid accountId, UpdatePlayerProfileRequestDto request);
        Task<PlayerStatsResponseDto?> GetPlayerStatsAsync(Guid accountId);
        Task<PlayerCurrencyApiResponseDto> GetCurrencyAsync(Guid accountId);
        Task<PlayerCurrencyApiResponseDto> AddCurrencyAsync(Guid accountId, CurrencyUpdateDto request);
        Task<PlayerCurrencyApiResponseDto> SpendCurrencyAsync(Guid accountId, CurrencyUpdateDto request);
        Task<PlayerCurrencyApiResponseDto> UpdateEnergyAsync(Guid accountId, int energyChange);
        Task<bool> HasProfileAsync(Guid accountId);
        Task<PlayerProfileApiResponseDto> AddExperienceAsync(Guid accountId, int amount);
    }
}
