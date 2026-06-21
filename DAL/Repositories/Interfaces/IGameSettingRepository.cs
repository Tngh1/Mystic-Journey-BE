using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IGameSettingRepository
    {
        Task<GameSetting?> GetGameSettingById(int id);
        Task<GameSetting?> GetByName(string name);
        Task<GameSetting> UpdateGameSetting(GameSetting setting);
        Task<(int TotalCount, List<GameSetting> Items)> GetSettingsPaged(int page, int pageSize, string? search);
    }
}
