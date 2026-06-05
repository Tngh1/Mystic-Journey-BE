using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IGachaBannerRepository
    {
        Task<GachaBanner?> GetGachaBannerById(int id);
        Task<GachaBanner?> GetGachaBannerByIdWithItems(int id);
        Task<List<GachaBanner>> GetAllGachaBanners();
        Task<List<GachaBanner>> GetActiveGachaBanners();
        Task<GachaBanner> CreateGachaBanner(GachaBanner banner);
        Task<GachaBanner> UpdateGachaBanner(GachaBanner banner);
        Task DeleteGachaBanner(int id);
        Task<GachaBannerItem> CreateBannerItem(GachaBannerItem item);
        Task<List<GachaBannerItem>> GetBannerItems(int bannerId);
        Task DeleteBannerItems(int bannerId);
        IQueryable<GachaBanner> GetGachaBannersQueryable();
        IQueryable<GachaBannerItem> GetBannerItemsQueryable();
    }
}
