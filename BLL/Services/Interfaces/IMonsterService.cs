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
        IQueryable<MonsterResponseDto> GetMonstersQueryable();
        IQueryable<MonsterDropResponseDto> GetMonsterDropsQueryable();
    }
}
