using BLL.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IMonsterService
    {
        Task<MonsterDetailResponseDto?> GetMonsterById(int id);
        Task<MonsterResponseDto> CreateMonster(CreateMonsterRequestDto request);
        Task<MonsterResponseDto> UpdateMonster(int id, UpdateMonsterRequestDto request);
        Task<MonsterDropResponseDto> AddMonsterDrop(int monsterId, CreateMonsterDropRequestDto request);
        Task<PagedResultDto<MonsterResponseDto>> GetMonstersPaged(int page, int pageSize, string? search, string? type, bool? isActive);
        Task<PagedResultDto<MonsterDropResponseDto>> GetMonsterDropsPaged(int page, int pageSize);
    }
}
