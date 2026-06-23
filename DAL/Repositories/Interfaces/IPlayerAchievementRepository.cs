using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IPlayerAchievementRepository
    {
        Task<List<PlayerAchievement>> GetByPlayerProfileId(int playerProfileId);
    }
}
