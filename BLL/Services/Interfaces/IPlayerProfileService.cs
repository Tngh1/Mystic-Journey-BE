using BLL.DTOs;
using DAL.Models;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Initializes a new default instance of the IPlayerProfileService class.
    public interface IPlayerProfileService
    {

        Task<PlayerProfileDetailResponseDto?> GetProfileById(int id);

        Task<PlayerProfileResponseDto?> GetByAccountIdAsync(int accountId);

        Task<PlayerProfileResponseDto> UpdateProfile(int id, UpdatePlayerProfileRequestDto request);

        Task<List<PlayerProfileResponseDto>> GetFriends(int playerProfileId);


        Task<PagedResultDto<PlayerProfileResponseDto>> GetProfilesPaged(int page, int pageSize, string? search, int? level);

        bool RecalculateEnergy(PlayerProfile profile);
        Task<PlayerProfileDetailResponseDto> ChangeName(int accountId, ChangeNameRequestDto request);
    }
}
