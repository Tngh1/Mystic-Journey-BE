using BLL.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IGachaBannerService
    {
        Task<GachaBannerDetailResponseDto?> GetBannerById(int id);
        Task<GachaBannerResponseDto> CreateBanner(CreateGachaBannerRequestDto request);
        Task<GachaBannerResponseDto> UpdateBanner(int id, UpdateGachaBannerRequestDto request);
        Task<GachaBannerItemResponseDto> AddBannerItem(int bannerId, CreateGachaBannerItemRequestDto request);
        IQueryable<GachaBannerResponseDto> GetBannersQueryable();
        IQueryable<GachaBannerItemResponseDto> GetBannerItemsQueryable();
    }
}
