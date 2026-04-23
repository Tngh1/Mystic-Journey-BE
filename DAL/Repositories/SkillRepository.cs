using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class SkillRepository : ISkillRepository
    {
        private readonly MysticJourneyDbContext _context;

        public SkillRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<Skill?> GetByIdAsync(Guid skillId)
        {
            return await _context.Skills
                .FirstOrDefaultAsync(s => s.Id == skillId && s.IsActive);
        }

        public async Task<List<Skill>> GetAllActiveAsync()
        {
            return await _context.Skills
                .Where(s => s.IsActive)
                .OrderBy(s => s.UnlockLevel)
                .ToListAsync();
        }

        public async Task<List<Skill>> GetByClassAsync(PlayerProfile.CharacterClass characterClass)
        {
            return await _context.Skills
                .Where(s => s.ClassRequirement == characterClass && s.IsActive)
                .OrderBy(s => s.UnlockLevel)
                .ToListAsync();
        }

        public async Task<List<Skill>> GetByCategoryAsync(Skill.SkillCategory category)
        {
            return await _context.Skills
                .Where(s => s.Type == category && s.IsActive)
                .OrderBy(s => s.UnlockLevel)
                .ToListAsync();
        }

        public async Task<List<Skill>> GetAvailableForLevelAsync(int playerLevel, PlayerProfile.CharacterClass characterClass)
        {
            return await _context.Skills
                .Where(s => s.UnlockLevel <= playerLevel &&
                            (s.ClassRequirement == characterClass || s.ClassRequirement == PlayerProfile.CharacterClass.Knight) &&
                            s.IsActive)
                .OrderBy(s => s.UnlockLevel)
                .ToListAsync();
        }

        public async Task<Skill> CreateAsync(Skill skill)
        {
            await _context.Skills.AddAsync(skill);
            await _context.SaveChangesAsync();
            return skill;
        }

        public async Task<Skill> UpdateAsync(Skill skill)
        {
            _context.Skills.Update(skill);
            await _context.SaveChangesAsync();
            return skill;
        }
    }
}
