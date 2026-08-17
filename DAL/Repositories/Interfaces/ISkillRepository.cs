using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Initializes a new default instance of the ISkillRepository class.
    public interface ISkillRepository
    {

        Task<PlayerSkill?> GetPlayerSkillById(int playerSkillId);

        Task<List<PlayerSkill>> GetPlayerSkillsByPlayerId(int playerProfileId);

        Task<List<Skill>> GetSkillsByNames(string[] names);

        Task<Skill?> GetSkillByName(string name);


        Task<Skill?> GetSkillById(int id);

        Task<List<Skill>> GetAllSkillsAsync();

        Task<(int TotalCount, List<Skill> Items)> GetSkillsPaged(int page, int pageSize, string? search, string? type, bool? isActive);

        Task<Skill> CreateSkill(Skill skill);

        Task<Skill> UpdateSkill(Skill skill);

        Task<PlayerSkill> CreatePlayerSkill(PlayerSkill playerSkill);

        Task<PlayerSkill> UpdatePlayerSkill(PlayerSkill playerSkill);

        Task DeletePlayerSkill(PlayerSkill playerSkill);
    }
}
