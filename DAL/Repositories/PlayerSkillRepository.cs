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
    public class PlayerSkillRepository : IPlayerSkillRepository
    {
        private readonly MysticJourneyDbContext _context;

        public PlayerSkillRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<PlayerSkill?> GetByIdAsync(Guid playerSkillId)
        {
            return await _context.PlayerSkills
                .FirstOrDefaultAsync(ps => ps.Id == playerSkillId);
        }

        public async Task<PlayerSkill?> GetByIdWithDetailsAsync(Guid playerSkillId)
        {
            return await _context.PlayerSkills
                .Include(ps => ps.Skill)
                .Include(ps => ps.PlayerProfile)
                .FirstOrDefaultAsync(ps => ps.Id == playerSkillId);
        }

        public async Task<List<PlayerSkill>> GetByPlayerProfileIdAsync(Guid playerProfileId)
        {
            return await _context.PlayerSkills
                .Include(ps => ps.Skill)
                .Where(ps => ps.PlayerProfileId == playerProfileId)
                .OrderByDescending(ps => ps.UnlockedAt)
                .ToListAsync();
        }

        public async Task<List<PlayerSkill>> GetEquippedSkillsAsync(Guid playerProfileId)
        {
            return await _context.PlayerSkills
                .Include(ps => ps.Skill)
                .Where(ps => ps.PlayerProfileId == playerProfileId && ps.IsEquipped)
                .ToListAsync();
        }

        public async Task<PlayerSkill?> GetByPlayerAndSkillAsync(Guid playerProfileId, Guid skillId)
        {
            return await _context.PlayerSkills
                .FirstOrDefaultAsync(ps => ps.PlayerProfileId == playerProfileId && ps.SkillId == skillId);
        }

        public async Task<PlayerSkill> CreateAsync(PlayerSkill playerSkill)
        {
            await _context.PlayerSkills.AddAsync(playerSkill);
            await _context.SaveChangesAsync();
            return playerSkill;
        }

        public async Task<PlayerSkill> UpdateAsync(PlayerSkill playerSkill)
        {
            _context.PlayerSkills.Update(playerSkill);
            await _context.SaveChangesAsync();
            return playerSkill;
        }

        public async Task<bool> HasSkillAsync(Guid playerProfileId, Guid skillId)
        {
            return await _context.PlayerSkills
                .AnyAsync(ps => ps.PlayerProfileId == playerProfileId && ps.SkillId == skillId);
        }
    }
}
