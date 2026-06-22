using BLL.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IDailyLoginRewardService
    {
        Task<DailyLoginRewardResponseDto> CreateDailyLoginReward(CreateDailyLoginRewardRequestDto request);
        Task<PagedResultDto<DailyLoginRewardResponseDto>> GetDailyLoginRewardsPaged(int page, int pageSize);
    }
}
