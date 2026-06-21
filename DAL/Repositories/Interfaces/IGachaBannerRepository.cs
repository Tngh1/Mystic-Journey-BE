using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IGachaBannerRepository
    {
        Task<GachaBanner?> GetGachaBannerById(int id);
        Task<GachaBanner?> GetGachaBannerByIdWithItems(int id);
        Task<GachaBanner> CreateGachaBanner(GachaBanner banner);
        Task<GachaBanner> UpdateGachaBanner(GachaBanner banner);
        Task<GachaBannerItem> CreateBannerItem(GachaBannerItem item);
        Task<List<GachaBannerItem>> GetBannerItems(int bannerId);
        Task<(int TotalCount, List<GachaBanner> Items)> GetBannersPaged(int page, int pageSize, string? search, string? type, bool? isActive);
        Task<(int TotalCount, List<GachaBannerItem> Items)> GetBannerItemsPaged(int page, int pageSize);
    }
}
