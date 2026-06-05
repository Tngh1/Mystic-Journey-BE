using BLL.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IAchievementService
    {
        Task<AchievementResponseDto?> GetAchievementById(int id);
        Task<AchievementResponseDto> CreateAchievement(CreateAchievementRequestDto request);
        Task<AchievementResponseDto> UpdateAchievement(int id, UpdateAchievementRequestDto request);
        IQueryable<AchievementResponseDto> GetAchievementsQueryable();
    }
}
