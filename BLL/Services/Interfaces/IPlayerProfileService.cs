using BLL.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IPlayerProfileService
    {
        Task<List<PlayerProfileResponseDto>> GetAllProfiles();
        Task<PlayerProfileDetailResponseDto?> GetProfileById(int id);
        Task<PlayerProfileResponseDto> UpdateProfile(int id, UpdatePlayerProfileRequestDto request);
        IQueryable<PlayerProfileResponseDto> GetProfilesQueryable();
    }
}
