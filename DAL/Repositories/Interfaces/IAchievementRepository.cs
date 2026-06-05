using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IAchievementRepository
    {
        Task<Achievement?> GetAchievementById(int id);
        Task<Achievement?> GetAchievementByIdWithReward(int id);
        Task<List<Achievement>> GetAllAchievements();
        Task<List<Achievement>> GetActiveAchievements();
        Task<Achievement> CreateAchievement(Achievement achievement);
        Task<Achievement> UpdateAchievement(Achievement achievement);
        Task<(int TotalCount, List<Achievement> Items)> GetAchievementsPaged(int page, int pageSize, string? search, string? type, bool? isActive);
    }
}
