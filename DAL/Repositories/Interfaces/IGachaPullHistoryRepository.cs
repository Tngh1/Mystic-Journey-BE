using DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IGachaPullHistoryRepository
    {
        Task<GachaPullHistory?> GetByIdAsync(Guid pullId);
        Task<List<GachaPullHistory>> GetByPlayerProfileIdAsync(Guid playerProfileId, int pageNumber = 1, int pageSize = 20);
        Task<List<GachaPullHistory>> GetByBannerAsync(Guid playerProfileId, Guid bannerId);
        Task<int> GetPullCountSinceLastFeaturedAsync(Guid playerProfileId, Guid bannerId);
        Task<GachaPullHistory> CreateAsync(GachaPullHistory pull);
        Task<int> GetTotalCountAsync(Guid playerProfileId);
    }
}
