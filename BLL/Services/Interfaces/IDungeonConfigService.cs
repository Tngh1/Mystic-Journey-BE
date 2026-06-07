using BLL.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IDungeonConfigService
    {
        Task<DungeonConfigResponseDto?> GetDungeonById(int id);
        Task<DungeonConfigResponseDto> CreateDungeon(CreateDungeonConfigRequestDto request);
        Task<DungeonConfigResponseDto> UpdateDungeon(int id, UpdateDungeonConfigRequestDto request);
        Task<PagedResultDto<DungeonConfigResponseDto>> GetDungeonsPaged(int page, int pageSize, string? search, string? type, bool? isActive);
    }
}
