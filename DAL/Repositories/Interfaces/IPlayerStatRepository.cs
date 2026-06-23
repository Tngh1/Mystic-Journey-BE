using DAL.Models;

namespace DAL.Repositories.Interfaces
{
    public interface IPlayerStatRepository
    {
        /// <summary>Gets the PlayerStat row for a given PlayerProfile, or null if not yet created.</summary>
        Task<PlayerStat?> GetByPlayerProfileId(int playerProfileId);

        /// <summary>Persists a brand-new PlayerStat row.</summary>
        Task<PlayerStat> Create(PlayerStat stat);

        /// <summary>Persists changes to an existing PlayerStat row.</summary>
        Task<PlayerStat> Update(PlayerStat stat);
    }
}
