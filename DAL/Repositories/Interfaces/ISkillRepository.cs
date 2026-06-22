using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface ISkillRepository
    {
        Task<Skill?> GetSkillById(int id);
        Task<(int TotalCount, List<Skill> Items)> GetSkillsPaged(int page, int pageSize, string? search, string? type, bool? isActive);
        Task<Skill> CreateSkill(Skill skill);
        Task<Skill> UpdateSkill(Skill skill);

        Task<PlayerSkill?> GetPlayerSkillById(int playerSkillId);
        Task<List<PlayerSkill>> GetPlayerSkillsByPlayerId(int playerProfileId);
        Task<PlayerSkill> CreatePlayerSkill(PlayerSkill playerSkill);
        Task<PlayerSkill> UpdatePlayerSkill(PlayerSkill playerSkill);
    }
}
