using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Initializes a new default instance of the IDungeonConfigService class.
    public interface IDungeonConfigService
    {

        Task<DungeonConfigResponseDto?> GetDungeonById(int id);

        Task<PagedResultDto<DungeonConfigResponseDto>> GetDungeonsPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? sortBy = null, string? sortOrder = null);


        Task<DungeonConfigResponseDto> UpdateDungeon(int id, UpdateDungeonConfigRequestDto request);

        Task<ChestItemResponseDto> AddChestItem(int dungeonId, CreateChestItemRequestDto request);
        Task<ChestItemResponseDto> UpdateChestItem(int dungeonId, int chestItemId, CreateChestItemRequestDto request);
        Task RemoveChestItem(int dungeonId, int chestItemId);
    }
}
