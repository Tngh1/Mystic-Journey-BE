using BLL.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IGameSettingService
    {
        Task<GameSettingResponseDto?> GetSettingById(int id);
        Task<GameSettingResponseDto?> GetSettingByKey(string key);
        Task<GameSettingResponseDto> CreateSetting(CreateGameSettingRequestDto request, Guid? updatedByAccountId = null);
        Task<GameSettingResponseDto> UpdateSetting(string key, UpdateGameSettingRequestDto request, Guid? updatedByAccountId = null);
        IQueryable<GameSettingResponseDto> GetSettingsQueryable();
    }
}
