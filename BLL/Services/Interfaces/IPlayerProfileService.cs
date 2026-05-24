using BLL.DTOs;
using System;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IPlayerProfileService
    {
        Task<PlayerProfileApiResponseDto> GetProfileByIdAsync(int id);
        Task<PlayerProfileApiResponseDto> GetProfileByAccountIdAsync(Guid accountId);
        Task<PlayerProfileApiResponseDto> CreateProfileAsync(CreatePlayerProfileRequestDto request);
        Task<PlayerProfileApiResponseDto> UpdateProfileAsync(int id, UpdatePlayerProfileRequestDto request);
    }
}
