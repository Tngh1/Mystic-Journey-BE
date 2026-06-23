using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
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

        public async Task<Skill?> GetSkillById(int id)
        {
            return await _context.Skills
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SkillId == id);
        }

        public async Task<(int TotalCount, List<Skill> Items)> GetSkillsPaged(int page, int pageSize, string? search, string? type, bool? isActive)
        {
            var query = _context.Skills.AsNoTracking();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(s => s.Name.Contains(search));
            if (!string.IsNullOrEmpty(type))
                query = query.Where(s => s.Type == type);
            if (isActive.HasValue)
                query = query.Where(s => s.IsActive == isActive.Value);

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (totalCount, items);
        }

        public async Task<Skill> CreateSkill(Skill skill)
        {
            await _context.Skills.AddAsync(skill);
            await _context.SaveChangesAsync();
            return skill;
        }

        public async Task<Skill> UpdateSkill(Skill skill)
        {
            _context.Skills.Update(skill);
            await _context.SaveChangesAsync();
            return skill;
        }

        public async Task<PlayerSkill?> GetPlayerSkillById(int playerSkillId)
        {
            return await _context.PlayerSkills
                .Include(ps => ps.Skill)
                .FirstOrDefaultAsync(ps => ps.PlayerSkillId == playerSkillId);
        }

        public async Task<List<PlayerSkill>> GetPlayerSkillsByPlayerId(int playerProfileId)
        {
            return await _context.PlayerSkills
                .Include(ps => ps.Skill)
                .Where(ps => ps.PlayerProfileId == playerProfileId)
                .ToListAsync();
        }

        public async Task<PlayerSkill> CreatePlayerSkill(PlayerSkill playerSkill)
        {
            await _context.PlayerSkills.AddAsync(playerSkill);
            await _context.SaveChangesAsync();
            return playerSkill;
        }

        public async Task<PlayerSkill> UpdatePlayerSkill(PlayerSkill playerSkill)
        {
            _context.PlayerSkills.Update(playerSkill);
            await _context.SaveChangesAsync();
            return playerSkill;
        }

        public async Task DeletePlayerSkill(PlayerSkill playerSkill)
        {
            _context.PlayerSkills.Remove(playerSkill);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Skill>> GetSkillsByNames(string[] names)
        {
            return await _context.Skills
                .Where(s => names.Contains(s.Name))
                .ToListAsync();
        }
    }
}
