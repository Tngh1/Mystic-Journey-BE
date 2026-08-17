using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    // Queries the database to retrieve i player quest repository records.
    public class PlayerQuestRepository : IPlayerQuestRepository
    {
        private readonly MysticJourneyDbContext _context;

        // Initializes a new instance of PlayerQuestRepository with dependencies: context.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public PlayerQuestRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        // Queries the database to retrieve get by player id records.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties; sorts records according to business ordering rules.
        // Returns the matching List<PlayerQuest entity result or default if not found.
        public async Task<List<PlayerQuest>> GetByPlayerId(int playerProfileId)
        {
            return await _context.PlayerQuests
                .Include(pq => pq.Quest)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(q => q!.RewardItem)
                .Include(pq => pq.Quest)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(q => q!.RewardItems)
                        .ThenInclude(r => r.Item)
                .Include(pq => pq.Quest)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(q => q!.RewardSkills)
                        .ThenInclude(r => r.Skill)
                .Include(pq => pq.Quest)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(q => q!.RewardSkill)
                .Where(pq => pq.PlayerProfileId == playerProfileId)  // Filter records matching the predicate
                .OrderByDescending(pq => pq.AcceptedAt)  // Sort results newest/highest first
                .AsSplitQuery()
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking()  // Disable EF Core change tracking for this read-only query
                .ToListAsync();  // Materialize the query into a list from the database
        }

        // Queries the database to retrieve get by player and quest records.
        // Query details: eagerly loads related entity navigation properties.
        // Returns the matching PlayerQuest? entity result or default if not found.
        public async Task<PlayerQuest?> GetByPlayerAndQuest(int playerProfileId, int questId)
        {
            return await _context.PlayerQuests
                .Include(pq => pq.Quest)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(q => q!.RewardItem)
                .Include(pq => pq.Quest)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(q => q!.RewardItems)
                        .ThenInclude(r => r.Item)
                .Include(pq => pq.Quest)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(q => q!.RewardSkills)
                        .ThenInclude(r => r.Skill)
                .Include(pq => pq.Quest)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(q => q!.RewardSkill)
                .AsSplitQuery()
                .FirstOrDefaultAsync(pq =>  // Fetch single matching record or null if not found
                    pq.PlayerProfileId == playerProfileId &&
                    pq.QuestId == questId);
        }

        // Performs database query and transactional persistence workflow for get by player and quest ids.
        // Query details: eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching List<PlayerQuest entity result or default if not found.
        public async Task<List<PlayerQuest>> GetByPlayerAndQuestIds(int playerProfileId, List<int> questIds)
        {
            return await _context.PlayerQuests
                .Include(pq => pq.Quest)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(q => q!.RewardItem)
                .Include(pq => pq.Quest)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(q => q!.RewardItems)
                        .ThenInclude(r => r.Item)
                .Include(pq => pq.Quest)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(q => q!.RewardSkills)
                        .ThenInclude(r => r.Skill)
                .Include(pq => pq.Quest)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(q => q!.RewardSkill)
                .Where(pq => pq.PlayerProfileId == playerProfileId && questIds.Contains(pq.QuestId))  // Filter records matching the predicate
                .AsSplitQuery()
                .ToListAsync();  // Materialize the query into a list from the database
        }

        // Persists state modifications to the database for create.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching PlayerQuest entity result or default if not found.
        public async Task<PlayerQuest> Create(PlayerQuest entity)
        {
            _context.PlayerQuests.Add(entity);
            try
            {
                await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
                return entity;
            }
            catch
            {
                _context.Entry(entity).State = EntityState.Detached;
                throw;
            }
        }

        // Per-frame update loop for PlayerQuestRepository.
        // Handles real-time input polling, smooth interpolations, cooldown timers, and UI updates.
        public async Task<PlayerQuest> Update(PlayerQuest entity)
        {
            _context.PlayerQuests.Update(entity);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return entity;
        }

        // Persists state modifications to the database for update range.
        public async Task UpdateRange(List<PlayerQuest> entities)
        {
            _context.PlayerQuests.UpdateRange(entities);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
        }
    }
}
