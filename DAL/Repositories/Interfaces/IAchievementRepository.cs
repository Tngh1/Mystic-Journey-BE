using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Initializes a new default instance of the IAchievementRepository class.
    public interface IAchievementRepository
    {

        Task<Achievement?> GetAchievementById(int id);

        Task<Achievement?> GetAchievementByIdWithReward(int id);

        Task<List<Achievement>> GetAllActiveAchievements();


        Task<Achievement> UpdateAchievement(Achievement achievement);

        Task<(int TotalCount, List<Achievement> Items)> GetAchievementsPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? sortBy = null, string? sortOrder = null);
    }
}
