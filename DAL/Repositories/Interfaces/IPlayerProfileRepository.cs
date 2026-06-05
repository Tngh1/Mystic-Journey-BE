using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IPlayerProfileRepository
    {
        Task<PlayerProfile?> GetPlayerProfileById(int id);
        Task<PlayerProfile?> GetPlayerProfileByIdWithStats(int id);
        Task<PlayerProfile?> GetByIdFull(int id);
        Task<List<PlayerProfile>> GetAllPlayerProfiles();
        Task<List<PlayerProfile>> GetAllPlayerProfilesWithAccounts();
        Task<PlayerProfile> CreatePlayerProfile(PlayerProfile profile);
        Task<PlayerProfile> UpdatePlayerProfile(PlayerProfile profile);
        Task<List<PlayerProfile>> Search(string? keyword = null, string? playerClass = null, bool? isBanned = null);
        Task<int> GetTotalPlayerProfilesCount();
        IQueryable<PlayerProfile> GetPlayerProfilesQueryable();
    }
}
