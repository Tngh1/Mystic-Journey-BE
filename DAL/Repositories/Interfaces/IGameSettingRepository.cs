using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IGameSettingRepository
    {
        Task<GameSetting?> GetGameSettingById(int id);
        Task<GameSetting?> GetByName(string name);
        Task<List<GameSetting>> GetAllGameSettings();
        Task<GameSetting> CreateGameSetting(GameSetting setting);
        Task<GameSetting> UpdateGameSetting(GameSetting setting);
        Task DeleteGameSetting(int id);
        IQueryable<GameSetting> GetGameSettingsQueryable();
    }
}
