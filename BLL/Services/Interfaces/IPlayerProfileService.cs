using BLL.DTOs;
using DAL.Models;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IPlayerProfileService
    {
        bool RecalculateEnergy(PlayerProfile profile);
        Task<PlayerProfileDetailResponseDto?> GetProfileById(int id);
        Task<PlayerProfileResponseDto> UpdateProfile(int id, UpdatePlayerProfileRequestDto request);
        Task<PagedResultDto<PlayerProfileResponseDto>> GetProfilesPaged(int page, int pageSize, string? search, int? level);
        Task<PlayerMeInventoryResponseDto> GetMeInventory(int playerProfileId);
        Task<PlayerMeSkillsResponseDto> GetMeSkills(int playerProfileId);
        Task<PlayerMeQuestsResponseDto> GetMeQuests(int playerProfileId);
        Task<PlayerMeAchievementsResponseDto> GetMeAchievements(int playerProfileId);
        Task<List<PlayerProfileResponseDto>> GetFriends(int playerProfileId);
    }
}
