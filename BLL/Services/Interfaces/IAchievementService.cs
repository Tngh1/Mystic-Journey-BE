using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Initializes a new default instance of the IAchievementService class.
    public interface IAchievementService
    {

        Task<PlayerMeAchievementsResponseDto> GetMeAchievements(int playerProfileId);

        Task<PlayerAchievementResponseDto> UnlockAchievement(int playerProfileId, int playerAchievementId);

        Task<AchievementResponseDto?> GetAchievementById(int id);


        Task<PagedResultDto<AchievementResponseDto>> GetAchievementsPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? sortBy = null, string? sortOrder = null);

        Task<AchievementResponseDto> UpdateAchievement(int id, UpdateAchievementRequestDto request);
    }
}
