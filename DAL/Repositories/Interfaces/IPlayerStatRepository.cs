using DAL.Models;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Initializes a new default instance of the IPlayerStatRepository class.
    public interface IPlayerStatRepository
    {

        Task<PlayerStat?> GetByPlayerProfileId(int playerProfileId);

        Task<PlayerStatsSnapshot?> GetSnapshotByPlayerProfileId(int playerProfileId);


        Task<PlayerStat> Create(PlayerStat stat);

        Task<PlayerStat> Update(PlayerStat stat);

        Task<PlayerStatsSnapshot> CreateSnapshot(PlayerStatsSnapshot snapshot);

        Task<PlayerStatsSnapshot> UpdateSnapshot(PlayerStatsSnapshot snapshot);
    }
}
