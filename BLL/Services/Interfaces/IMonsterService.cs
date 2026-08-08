using BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Quản lý monsters (quái vật) và monster spawns (vị trí spawn).
    // Game APIs: Khám phá, đánh bại, xem catalog, xem spawns.
    // Admin APIs: Tạo, cập nhật monster và spawns.
    public interface IMonsterService
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy thông tin monster theo ID.
        Task<MonsterDetailResponseDto?> GetMonsterById(int id);

        // Lấy thông tin monster cho player cụ thể (có trạng thái khám phá).
        Task<MonsterDetailResponseDto?> GetMonsterForPlayer(int id, int playerProfileId);

        // Lấy danh sách tất cả monsters có phân trang và lọc.
        Task<PagedResultDto<MonsterResponseDto>> GetMonstersPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? sortBy = null, string? sortOrder = null);

        // Lấy catalog monsters đã khám phá của player.
        Task<PagedResultDto<PlayerMonsterCatalogItemDto>> GetMonsterCatalogForPlayer(int playerProfileId, int page, int pageSize, string? search, string? type);

        // Lấy danh sách vị trí spawn monsters theo map.
        Task<List<MonsterSpawnResponseDto>> GetSpawnsForPlayer(int playerProfileId, string mapName, string? regionName, int? dungeonId);

        // Lấy danh sách monster drops có phân trang.
        Task<PagedResultDto<MonsterDropResponseDto>> GetMonsterDropsPaged(int page, int pageSize);

        // Khám phá monster (thêm vào catalog của player).
        Task<PlayerMonsterCatalogItemDto> DiscoverMonster(int playerProfileId, int monsterId);

        // Đánh bại monster, nhận XP và gold.
        Task<MonsterDefeatResponseDto> DefeatMonster(int playerProfileId, int monsterId, MonsterDefeatRequestDto? request);

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Cập nhật monster hiện có.
        Task<MonsterResponseDto> UpdateMonster(int id, UpdateMonsterRequestDto request);

        // Thêm drop cho monster.
        Task<MonsterDropResponseDto> AddMonsterDrop(int monsterId, CreateMonsterDropRequestDto request);

        // Lấy danh sách spawns của một monster (Admin).
        Task<List<MonsterSpawnResponseDto>> GetSpawnsByMonsterId(int monsterId);

        // Tạo spawn mới cho monster.
        Task<MonsterSpawnResponseDto> CreateSpawn(CreateMonsterSpawnRequestDto request);

        // Cập nhật spawn
        Task<MonsterSpawnResponseDto> UpdateSpawn(int spawnId, UpdateMonsterSpawnRequestDto request);

        // Xoá spawn.
        Task DeleteSpawn(int spawnId);

        // Lấy danh sách spawns của một dungeon (Admin).
        Task<List<MonsterSpawnResponseDto>> GetSpawnsByDungeonId(int dungeonId);
    }
}
