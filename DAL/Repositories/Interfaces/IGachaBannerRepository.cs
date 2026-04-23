using DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IGachaBannerRepository
    {
        Task<GachaBanner?> GetByIdAsync(Guid bannerId);
        Task<GachaBanner?> GetByIdWithItemsAsync(Guid bannerId);
        Task<List<GachaBanner>> GetAllActiveAsync();
        Task<List<GachaBanner>> GetAvailableNowAsync();
        Task<GachaBanner> CreateAsync(GachaBanner banner);
        Task<GachaBanner> UpdateAsync(GachaBanner banner);
    }
}
