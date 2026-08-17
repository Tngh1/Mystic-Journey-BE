using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    // Queries the database to retrieve i skill repository records.
    public class SkillRepository : ISkillRepository
    {
        private readonly MysticJourneyDbContext _context;

        // Initializes a new instance of SkillRepository with dependencies: context.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public SkillRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        // Queries the database to retrieve get skill by id records.
        // Query details: uses AsNoTracking() for read-only query optimization; applies pagination offset and limit parameters.
        // Returns the matching Skill? entity result or default if not found.
        public async Task<Skill?> GetSkillById(int id)
        {
            return await _context.Skills
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking()  // Disable EF Core change tracking for this read-only query
                .FirstOrDefaultAsync(s => s.SkillId == id);  // Fetch single matching record or null if not found
        }

        // Performs database query and transactional persistence workflow for get skills paged.
        // Query details: uses AsNoTracking() for read-only query optimization; commits entity state changes via EF Core SaveChangesAsync; applies pagination offset and limit parameters.
        public async Task<(int TotalCount, List<Skill> Items)> GetSkillsPaged(int page, int pageSize, string? search, string? type, bool? isActive)
        {
            // Execute this query without change tracking because the returned entities are read-only.
            var query = _context.Skills.AsNoTracking();  // Disable EF Core change tracking for this read-only query

            if (!string.IsNullOrEmpty(search))
                query = query.Where(s => s.Name.Contains(search));  // Filter records matching the predicate
            if (!string.IsNullOrEmpty(type))
                query = query.Where(s => s.Type == type);  // Filter records matching the predicate
            if (isActive.HasValue)
                query = query.Where(s => s.IsActive == isActive.Value);  // Filter records matching the predicate

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();  // Materialize the query into a list from the database

            return (totalCount, items);
        }

        // Persists state modifications to the database for create skill.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching Skill entity result or default if not found.
        public async Task<Skill> CreateSkill(Skill skill)
        {
            await _context.Skills.AddAsync(skill);  // Stage new entity for insertion in the next SaveChanges call
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return skill;
        }

        // Performs database query and transactional persistence workflow for update skill.
        // Query details: eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching Skill entity result or default if not found.
        public async Task<Skill> UpdateSkill(Skill skill)
        {
            _context.Skills.Update(skill);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return skill;
        }

        // Queries the database to retrieve get player skill by id records.
        // Query details: eagerly loads related entity navigation properties.
        // Returns the matching PlayerSkill? entity result or default if not found.
        public async Task<PlayerSkill?> GetPlayerSkillById(int playerSkillId)
        {
            return await _context.PlayerSkills
                .Include(ps => ps.Skill)  // Eagerly load related navigation entities to avoid N+1 queries
                .FirstOrDefaultAsync(ps => ps.PlayerSkillId == playerSkillId);  // Fetch single matching record or null if not found
        }

        // Performs database query and transactional persistence workflow for get player skills by player id.
        // Query details: eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching List<PlayerSkill entity result or default if not found.
        public async Task<List<PlayerSkill>> GetPlayerSkillsByPlayerId(int playerProfileId)
        {
            return await _context.PlayerSkills
                .Include(ps => ps.Skill)  // Eagerly load related navigation entities to avoid N+1 queries
                .Where(ps => ps.PlayerProfileId == playerProfileId)  // Filter records matching the predicate
                .ToListAsync();  // Materialize the query into a list from the database
        }

        // Persists state modifications to the database for create player skill.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching PlayerSkill entity result or default if not found.
        public async Task<PlayerSkill> CreatePlayerSkill(PlayerSkill playerSkill)
        {
            await _context.PlayerSkills.AddAsync(playerSkill);  // Stage new entity for insertion in the next SaveChanges call
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return playerSkill;
        }

        // Persists state modifications to the database for update player skill.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching PlayerSkill entity result or default if not found.
        public async Task<PlayerSkill> UpdatePlayerSkill(PlayerSkill playerSkill)
        {
            _context.PlayerSkills.Update(playerSkill);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return playerSkill;
        }

        // Performs database query and transactional persistence workflow for delete player skill.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        public async Task DeletePlayerSkill(PlayerSkill playerSkill)
        {
            _context.PlayerSkills.Remove(playerSkill);  // Mark entity for deletion in the next SaveChanges call
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
        }

        // Queries the database to retrieve get skills by names records.
        // Query details: uses AsNoTracking() for read-only query optimization.
        // Returns the matching List<Skill entity result or default if not found.
        public async Task<List<Skill>> GetSkillsByNames(string[] names)
        {
            return await _context.Skills
                .Where(s => names.Contains(s.Name))  // Filter records matching the predicate
                .ToListAsync();  // Materialize the query into a list from the database
        }

        // Queries the database to retrieve get skill by name records.
        // Query details: uses AsNoTracking() for read-only query optimization.
        // Returns the matching Skill? entity result or default if not found.
        public async Task<Skill?> GetSkillByName(string name)
        {
            return await _context.Skills
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking()  // Disable EF Core change tracking for this read-only query
                .FirstOrDefaultAsync(s => s.Name == name);  // Fetch single matching record or null if not found
        }

        // Load all skills async; it materializes the query results.
        public async Task<List<Skill>> GetAllSkillsAsync()
        {
            return await _context.Skills
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking()  // Disable EF Core change tracking for this read-only query
                .ToListAsync();  // Materialize the query into a list from the database
        }
    }
}
