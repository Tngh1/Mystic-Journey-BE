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
    // Queries the database to retrieve i quest repository records.
    public class QuestRepository : IQuestRepository
    {
        private readonly MysticJourneyDbContext _context;

        // Initializes a new instance of QuestRepository with dependencies: context.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public QuestRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        // Queries the database to retrieve get quest by id records.
        // Query details: eagerly loads related entity navigation properties.
        // Returns the matching Quest? entity result or default if not found.
        public async Task<Quest?> GetQuestById(int id)
        {
            return await _context.Quests
                .FirstOrDefaultAsync(q => q.QuestId == id);  // Fetch single matching record or null if not found
        }

        // Queries the database to retrieve get by id with reward records.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties.
        // Returns the matching Quest? entity result or default if not found.
        public async Task<Quest?> GetByIdWithReward(int id)
        {
            return await _context.Quests
                .Include(q => q.RewardItem)  // Eagerly load related navigation entities to avoid N+1 queries
                .Include(q => q.RewardItems)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(r => r.Item)
                .Include(q => q.RewardSkills)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(r => r.Skill)
                .Include(q => q.RewardSkill)  // Eagerly load related navigation entities to avoid N+1 queries
                .FirstOrDefaultAsync(q => q.QuestId == id);  // Fetch single matching record or null if not found
        }

        // Load active quests; it filters the eligible records and materializes the query results.
        public async Task<List<Quest>> GetActiveQuests()
        {
            return await _context.Quests
                .Include(q => q.RewardItem)  // Eagerly load related navigation entities to avoid N+1 queries
                .Include(q => q.RewardItems)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(r => r.Item)
                .Include(q => q.RewardSkills)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(r => r.Skill)
                .Include(q => q.RewardSkill)  // Eagerly load related navigation entities to avoid N+1 queries
                .Where(q => q.IsActive)  // Filter records matching the predicate
                .AsSplitQuery()
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking()  // Disable EF Core change tracking for this read-only query
                .ToListAsync();  // Materialize the query into a list from the database
        }

        // Persists state modifications to the database for add quest.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching Quest entity result or default if not found.
        public async Task<Quest> AddQuest(Quest quest)
        {
            _context.Quests.Add(quest);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return quest;
        }

        // Performs database query and transactional persistence workflow for update quest.
        // Query details: eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync; sorts records according to business ordering rules.
        // Returns the matching Quest entity result or default if not found.
        public async Task<Quest> UpdateQuest(Quest quest)
        {
            _context.Quests.Update(quest);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return quest;
        }

        // Queries the database to retrieve get quest dialogue by quest id records.
        // Query details: eagerly loads related entity navigation properties; sorts records according to business ordering rules.
        // Returns the matching NPCDialogue? entity result or default if not found.
        public async Task<NPCDialogue?> GetQuestDialogueByQuestId(int questId)
        {
            return await _context.NPCDialogues
                .Include(d => d.NPC)  // Eagerly load related navigation entities to avoid N+1 queries
                .Include(d => d.LinkedQuest)  // Eagerly load related navigation entities to avoid N+1 queries
                .Where(d => d.LinkedQuestId == questId && d.ResponseType == "Quest")  // Filter records matching the predicate
                .OrderByDescending(d => d.IsActive)  // Sort results newest/highest first
                .ThenBy(d => d.DisplayOrder)
                .ThenBy(d => d.NPCDialogueId)
                .FirstOrDefaultAsync();  // Fetch single matching record or null if not found
        }

        // Queries the database to retrieve get npc by name and map records.
        // Query details: uses AsNoTracking() for read-only query optimization; sorts records according to business ordering rules.
        // Returns the matching NPC? entity result or default if not found.
        public async Task<NPC?> GetNpcByNameAndMap(string? npcName, string mapName)
        {
            if (string.IsNullOrWhiteSpace(npcName))  // Mandatory string argument is blank — fail fast
                return null;

            var normalizedName = npcName.Trim();
            return await _context.NPCs
                .Where(n => n.Name == normalizedName && (n.MapName == mapName || (mapName.StartsWith("Autumn") && n.MapName.StartsWith("Autumn"))))  // Filter records matching the predicate
                .OrderByDescending(n => n.IsActive)  // Sort results newest/highest first
                .FirstOrDefaultAsync()  // Fetch single matching record or null if not found
                ?? await _context.NPCs
                    .Where(n => n.Name == normalizedName)  // Filter records matching the predicate
                    .OrderByDescending(n => n.IsActive)  // Sort results newest/highest first
                    .FirstOrDefaultAsync();  // Fetch single matching record or null if not found
        }

        // Queries the database to retrieve get quest npc options records.
        // Query details: uses AsNoTracking() for read-only query optimization.
        // Returns the matching List<NPC entity result or default if not found.
        public async Task<List<NPC>> GetQuestNpcOptions(string? mapName)
        {
            var query = _context.NPCs
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking()  // Disable EF Core change tracking for this read-only query
                .Where(n => n.IsActive);  // Filter records matching the predicate

            if (!string.IsNullOrWhiteSpace(mapName))
            {
                var normalizedMapName = mapName.Trim();
                if (normalizedMapName.Equals("AutumnTown", StringComparison.OrdinalIgnoreCase) ||
                    normalizedMapName.Equals("AutumnPumpkin", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(n => n.MapName == "AutumnTown" || n.MapName == "AutumnPumpkin");  // Filter records matching the predicate
                }
                else
                {
                    query = query.Where(n => n.MapName == normalizedMapName);  // Filter records matching the predicate
                }
            }

            return await query
                .OrderBy(n => n.MapName)  // Sort results oldest/lowest first
                .ThenBy(n => n.Name)
                .ThenBy(n => n.NPCId)
                .Take(200)  // Apply pagination limit — cap result set size
                .ToListAsync();  // Materialize the query into a list from the database
        }

        // Queries the database to retrieve add quest dialogue records.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties; sorts records according to business ordering rules; applies pagination offset and limit parameters.
        public void AddQuestDialogue(NPCDialogue dialogue)
        {
            _context.NPCDialogues.Add(dialogue);
        }

        // Queries the database to retrieve get quests paged records.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties; sorts records according to business ordering rules; applies pagination offset and limit parameters.
        public async Task<(int TotalCount, List<Quest> Items)> GetQuestsPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? mapName, string? sortBy = null, string? sortOrder = null)
        {
            // Execute this query without change tracking because the returned entities are read-only.
            var filtered = _context.Quests.AsNoTracking();  // Disable EF Core change tracking for this read-only query

            if (!string.IsNullOrEmpty(search))
            {
                filtered = filtered.Where(x => x.Title.Contains(search));  // Filter records matching the predicate
            }
            if (!string.IsNullOrEmpty(type))
            {
                filtered = filtered.Where(x => x.Type == type);  // Filter records matching the predicate
            }
            if (isActive.HasValue)
            {
                filtered = filtered.Where(x => x.IsActive == isActive.Value);  // Filter records matching the predicate
            }
            if (!string.IsNullOrEmpty(mapName))
            {
                if (mapName.Equals("AutumnTown", StringComparison.OrdinalIgnoreCase) ||
                    mapName.Equals("AutumnPumpkin", StringComparison.OrdinalIgnoreCase))
                {
                    filtered = filtered.Where(x => x.MapName == "AutumnTown" || x.MapName == "AutumnPumpkin");  // Filter records matching the predicate
                }
                else
                {
                    filtered = filtered.Where(x => x.MapName == mapName);  // Filter records matching the predicate
                }
            }

            int totalCount = await filtered.CountAsync();

            var query = filtered
                .Include(q => q.RewardItem)  // Eagerly load related navigation entities to avoid N+1 queries
                .Include(q => q.RewardItems)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(r => r.Item)
                .Include(q => q.RewardSkills)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(r => r.Skill)
                .Include(q => q.RewardSkill)  // Eagerly load related navigation entities to avoid N+1 queries
                .AsSplitQuery();

            bool desc = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);
            IQueryable<Quest> ordered = (sortBy?.ToLowerInvariant()) switch
            {
                "title" => desc ? query.OrderByDescending(x => x.Title) : query.OrderBy(x => x.Title),  // Sort results newest/highest first
                "type" => desc ? query.OrderByDescending(x => x.Type) : query.OrderBy(x => x.Type),  // Sort results newest/highest first
                "requiredlevel" => desc ? query.OrderByDescending(x => x.RequiredLevel) : query.OrderBy(x => x.RequiredLevel),  // Sort results newest/highest first
                "rewardgold" => desc ? query.OrderByDescending(x => x.RewardGold) : query.OrderBy(x => x.RewardGold),  // Sort results newest/highest first
                "rewardexp" => desc ? query.OrderByDescending(x => x.RewardExperience) : query.OrderBy(x => x.RewardExperience),  // Sort results newest/highest first
                "mapname" => desc ? query.OrderByDescending(x => x.MapName) : query.OrderBy(x => x.MapName),  // Sort results newest/highest first
                "isactive" => desc ? query.OrderByDescending(x => x.IsActive) : query.OrderBy(x => x.IsActive),  // Sort results newest/highest first
                _ => desc ? query.OrderByDescending(x => x.QuestId) : query.OrderBy(x => x.QuestId),  // Sort results newest/highest first
            };

            var items = await ordered.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();  // Materialize the query into a list from the database

            return (totalCount, items);
        }
    }
}
