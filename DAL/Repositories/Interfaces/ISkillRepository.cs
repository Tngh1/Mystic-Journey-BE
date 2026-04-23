using DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface ISkillRepository
    {
        Task<Skill?> GetByIdAsync(Guid skillId);
        Task<List<Skill>> GetAllActiveAsync();
        Task<List<Skill>> GetByClassAsync(PlayerProfile.CharacterClass characterClass);
        Task<List<Skill>> GetByCategoryAsync(Skill.SkillCategory category);
        Task<List<Skill>> GetAvailableForLevelAsync(int playerLevel, PlayerProfile.CharacterClass characterClass);
        Task<Skill> CreateAsync(Skill skill);
        Task<Skill> UpdateAsync(Skill skill);
    }
}
