using DAL.Models;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Initializes a new default instance of the IPlayerAchievementRepository class.
    public interface IPlayerAchievementRepository
    {

        Task<List<PlayerAchievement>> GetByPlayerProfileId(int playerProfileId);

        Task<PlayerAchievement?> GetByIdWithAchievement(int playerAchievementId);

        Task<PlayerAchievement> Update(PlayerAchievement playerAchievement);

        Task UpdateRange(IEnumerable<PlayerAchievement> achievements);

        Task AddRange(IEnumerable<PlayerAchievement> achievements);
    }
}
