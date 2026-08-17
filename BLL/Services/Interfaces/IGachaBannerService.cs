using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Initializes a new default instance of the IGachaBannerService class.
    public interface IGachaBannerService
    {

        Task<GachaBannerDetailResponseDto?> GetBannerById(int id);

        Task<PagedResultDto<GachaBannerResponseDto>> GetBannersPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? sortBy = null, string? sortOrder = null);

        Task<PagedResultDto<GachaBannerItemResponseDto>> GetBannerItemsPaged(int page, int pageSize);

        Task<MultiPullResultDto> Pull(int playerProfileId, int bannerId, GachaPullRequestDto request);

        Task<PagedResultDto<GachaPullHistoryResponseDto>> GetHistoryPaged(int playerProfileId, int page, int pageSize);


        Task<GachaBannerResponseDto> CreateBanner(CreateGachaBannerRequestDto request);

        Task<GachaBannerResponseDto> UpdateBanner(int id, UpdateGachaBannerRequestDto request);

        Task<GachaBannerItemResponseDto> AddBannerItem(int bannerId, CreateGachaBannerItemRequestDto request);

        Task<bool> RemoveBannerItem(int bannerId, int bannerItemId);

        Task<PagedResultDto<GachaPullHistoryResponseDto>> GetAllHistoryPaged(int page, int pageSize, int? bannerId, string? rarity);

        Task<PlayerGachaStatsDto?> GetPlayerGachaStats(int playerProfileId);
    }
}
