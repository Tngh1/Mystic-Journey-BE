using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Initializes a new default instance of the IDailyLoginRewardRepository class.
    public interface IDailyLoginRewardRepository
    {

        Task<DailyLoginReward?> GetDailyLoginRewardById(int id);

        Task<DailyLoginReward?> GetByDayAndMonth(int dayNumber, int month, int year);

        Task<DailyLoginReward?> GetDefaultByDayNumber(int dayNumber);

        Task<List<DailyLoginReward>> GetOverridesByMonth(int month, int year);

        Task<List<DailyLoginReward>> GetAllDefaults();


        Task<(int TotalCount, List<DailyLoginReward> Items)> GetDailyLoginRewardsPaged(
            int page, int pageSize, int? month = null, int? year = null);

        Task<DailyLoginReward> CreateDailyLoginReward(DailyLoginReward reward);

        Task<DailyLoginReward> UpdateDailyLoginReward(DailyLoginReward reward);

        Task DeleteDailyLoginReward(int id);
    }
}
