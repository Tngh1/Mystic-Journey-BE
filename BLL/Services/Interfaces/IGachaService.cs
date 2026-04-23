using BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IGachaService
    {
        Task<GachaBannerListResponseDto> GetAllBannersAsync();
        Task<GachaBannerListResponseDto> GetAvailableBannersAsync();
        Task<GachaApiResponseDto> GetBannerByIdAsync(Guid bannerId);
        Task<GachaApiResponseDto> PullGachaAsync(Guid accountId, GachaPullRequestDto request);
        Task<GachaHistoryListResponseDto> GetPullHistoryAsync(Guid accountId, int pageNumber = 1, int pageSize = 20);
    }
}
