using DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IPlayerProfileRepository
    {
        Task<PlayerProfile?> GetByIdAsync(Guid profileId);
        Task<PlayerProfile?> GetByAccountIdAsync(Guid accountId);
        Task<PlayerProfile?> GetByIdWithDetailsAsync(Guid profileId);
        Task<PlayerProfile?> GetByAccountIdWithDetailsAsync(Guid accountId);
        Task<PlayerProfile> CreateAsync(PlayerProfile profile);
        Task<PlayerProfile> UpdateAsync(PlayerProfile profile);
        Task<bool> ExistsAsync(Guid accountId);
        Task<PlayerStat?> GetStatsByProfileIdAsync(Guid profileId);
        Task<PlayerStat> UpdateStatsAsync(PlayerStat stats);
        Task UpdateCurrencyAsync(Guid profileId, decimal? gold = null, decimal? gems = null, int? energy = null);
    }
}
