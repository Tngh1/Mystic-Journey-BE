using BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Initializes a new default instance of the IMonsterService class.
    public interface IMonsterService
    {

        Task<MonsterDetailResponseDto?> GetMonsterById(int id);

        Task<MonsterDetailResponseDto?> GetMonsterForPlayer(int id, int playerProfileId);

        Task<PagedResultDto<MonsterResponseDto>> GetMonstersPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? sortBy = null, string? sortOrder = null);

        Task<PagedResultDto<PlayerMonsterCatalogItemDto>> GetMonsterCatalogForPlayer(int playerProfileId, int page, int pageSize, string? search, string? type);

        Task<List<MonsterSpawnResponseDto>> GetSpawnsForPlayer(int playerProfileId, string mapName, string? regionName, int? dungeonId);

        Task<PagedResultDto<MonsterDropResponseDto>> GetMonsterDropsPaged(int page, int pageSize);

        Task<PlayerMonsterCatalogItemDto> DiscoverMonster(int playerProfileId, int monsterId);

        Task<MonsterDefeatResponseDto> DefeatMonster(int playerProfileId, int monsterId, MonsterDefeatRequestDto? request);


        Task<MonsterResponseDto> UpdateMonster(int id, UpdateMonsterRequestDto request);

        Task<MonsterDropResponseDto> AddMonsterDrop(int monsterId, CreateMonsterDropRequestDto request);

        Task<List<MonsterSpawnResponseDto>> GetSpawnsByMonsterId(int monsterId);

        Task<MonsterSpawnResponseDto> CreateSpawn(CreateMonsterSpawnRequestDto request);

        Task<MonsterSpawnResponseDto> UpdateSpawn(int spawnId, UpdateMonsterSpawnRequestDto request);

        Task DeleteSpawn(int spawnId);

        Task<List<MonsterSpawnResponseDto>> GetSpawnsByDungeonId(int dungeonId);
    }
}
