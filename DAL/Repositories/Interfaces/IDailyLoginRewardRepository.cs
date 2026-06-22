using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IDailyLoginRewardRepository
    {
        Task<(int TotalCount, List<DailyLoginReward> Items)> GetDailyLoginRewardsPaged(int page, int pageSize);
        Task<DailyLoginReward?> GetDailyLoginRewardById(int id);
        Task<DailyLoginReward?> GetDailyLoginRewardByDayNumber(int dayNumber);
        Task<DailyLoginReward> CreateDailyLoginReward(DailyLoginReward reward);
        Task<DailyLoginReward> UpdateDailyLoginReward(DailyLoginReward reward);
    }
}
