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
        Task<PagedResultDto<GachaBannerResponseDto>> GetBannersPaged(int page, int pageSize, string? search, string? type, bool? isActive);
        Task<PagedResultDto<GachaBannerItemResponseDto>> GetBannerItemsPaged(int page, int pageSize);
    }
}
