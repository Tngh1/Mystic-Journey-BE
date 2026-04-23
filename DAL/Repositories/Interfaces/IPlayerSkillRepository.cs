using DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IPlayerSkillRepository
    {
        Task<PlayerSkill?> GetByIdAsync(Guid playerSkillId);
        Task<PlayerSkill?> GetByIdWithDetailsAsync(Guid playerSkillId);
        Task<List<PlayerSkill>> GetByPlayerProfileIdAsync(Guid playerProfileId);
        Task<List<PlayerSkill>> GetEquippedSkillsAsync(Guid playerProfileId);
        Task<PlayerSkill?> GetByPlayerAndSkillAsync(Guid playerProfileId, Guid skillId);
        Task<PlayerSkill> CreateAsync(PlayerSkill playerSkill);
        Task<PlayerSkill> UpdateAsync(PlayerSkill playerSkill);
        Task<bool> HasSkillAsync(Guid playerProfileId, Guid skillId);
    }
}
