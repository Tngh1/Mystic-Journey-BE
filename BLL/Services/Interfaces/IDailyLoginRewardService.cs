using BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Initializes a new default instance of the IDailyLoginRewardService class.
    public interface IDailyLoginRewardService
    {

        Task<PagedResultDto<DailyLoginRewardResponseDto>> GetDailyLoginRewardsPaged(
            int page, int pageSize, int? month = null, int? year = null);

        Task<List<DailyLoginRewardResponseDto>> GetCurrentMonthRewards(int? month = null, int? year = null);


        Task<DailyLoginRewardResponseDto?> GetDailyLoginRewardById(int id);

        Task<List<DailyLoginRewardResponseDto>> GetRewardsByMonth(int? month, int? year);

        Task<DailyLoginRewardResponseDto> CreateDailyLoginReward(CreateDailyLoginRewardRequestDto request);

        Task<DailyLoginRewardResponseDto> UpdateDailyLoginReward(int id, UpdateDailyLoginRewardRequestDto request);

        Task DeleteDailyLoginReward(int id);
    }
}
