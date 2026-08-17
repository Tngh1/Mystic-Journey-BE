using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Initializes a new default instance of the IPlayerProfileRepository class.
    public interface IPlayerProfileRepository
    {

        Task<PlayerProfile?> GetPlayerProfileById(int id);

        Task<PlayerProfile?> GetPlayerProfileByIdWithStats(int id);

        Task<PlayerProfile?> GetByIdFull(int id);

        Task<PlayerProfile?> GetByAccountId(int accountId);

        Task<PlayerProfile?> GetPlayerProfileByName(string playerName);


        Task<List<PlayerProfile>> GetAllPlayerProfiles();

        Task<PlayerProfile> CreatePlayerProfile(PlayerProfile profile);

        Task<PlayerProfile> UpdatePlayerProfile(PlayerProfile profile);

        Task<List<PlayerProfile>> Search(string? keyword = null, string? playerClass = null);

        Task<int> GetTotalPlayerProfilesCount();

        Task<(int TotalCount, List<PlayerProfile> Items)> GetProfilesPaged(int page, int pageSize, string? search, int? level);
    }
}
