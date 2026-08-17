using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    // Queries the database to retrieve i world repository records.
    public class WorldRepository : IWorldRepository
    {
        private readonly MysticJourneyDbContext _context;

        // Initializes a new instance of WorldRepository with dependencies: context.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public WorldRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }


        // Queries the database to retrieve get npcs by map name records.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties; sorts records according to business ordering rules.
        // Returns the matching List<NPC entity result or default if not found.
        public async Task<List<NPC>> GetNpcsByMapName(string mapName, int take)
        {
            return await _context.NPCs
                .Include(n => n.Dialogues.Where(d => d.IsActive))  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(d => d.LinkedQuest)
                .Include(n => n.Dialogues.Where(d => d.IsActive))  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(d => d.LinkedShopItem)
                        .ThenInclude(si => si!.Item)
                .Where(n => n.IsActive && n.MapName == mapName)  // Filter records matching the predicate
                .OrderBy(n => n.NPCId)  // Sort results oldest/lowest first
                .Take(take)  // Apply pagination limit — cap result set size
                .ToListAsync();  // Materialize the query into a list from the database
        }

        // Load all npc map names; it filters the eligible records, projects records into the output shape, and materializes the query results.
        public async Task<List<string>> GetAllNpcMapNames()
        {
            return await _context.NPCs
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking()  // Disable EF Core change tracking for this read-only query
                .Where(n => n.IsActive)  // Filter records matching the predicate
                .Select(n => n.MapName)
                .ToListAsync();  // Materialize the query into a list from the database
        }

        // Queries the database to retrieve get npc by id records.
        // Query details: eagerly loads related entity navigation properties.
        // Returns the matching NPC? entity result or default if not found.
        public async Task<NPC?> GetNpcById(int npcId)
        {
            return await _context.NPCs
                .Include(n => n.Dialogues.Where(d => d.IsActive))  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(d => d.LinkedQuest)
                .Include(n => n.Dialogues.Where(d => d.IsActive))  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(d => d.LinkedShopItem)
                        .ThenInclude(si => si!.Item)
                .FirstOrDefaultAsync(n => n.NPCId == npcId && n.IsActive);  // Fetch single matching record or null if not found
        }

        // Queries the database to retrieve is quest linked to npc records.
        // Query details: eagerly loads related entity navigation properties.
        // Returns true if the operation succeeded or record exists; otherwise false.
        public async Task<bool> IsQuestLinkedToNpc(int npcId, int questId)
        {
            return await _context.NPCDialogues
                .AnyAsync(d => d.NPCId == npcId && d.LinkedQuestId == questId && d.IsActive);  // Check existence without loading the full entity
        }


        // Queries the database to retrieve get chest by id records.
        // Query details: eagerly loads related entity navigation properties.
        // Returns the matching Chest? entity result or default if not found.
        public async Task<Chest?> GetChestById(int chestId)
        {
            return await _context.Chests
                .Include(c => c.ChestItems)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(ci => ci.Item)
                .FirstOrDefaultAsync(c => c.ChestId == chestId && c.IsActive);  // Fetch single matching record or null if not found
        }

        // Performs database query and transactional persistence workflow for get player chest.
        // Query details: eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching PlayerChest? entity result or default if not found.
        public async Task<PlayerChest?> GetPlayerChest(int playerChestId, int playerProfileId)
        {
            return await _context.PlayerChests
                .Include(pc => pc.Chest)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(c => c!.ChestItems)
                        .ThenInclude(ci => ci.Item)
                .FirstOrDefaultAsync(pc =>  // Fetch single matching record or null if not found
                    pc.PlayerChestId == playerChestId &&
                    pc.PlayerProfileId == playerProfileId);
        }

        // Persists state modifications to the database for create player chest.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching PlayerChest entity result or default if not found.
        public async Task<PlayerChest> CreatePlayerChest(PlayerChest playerChest)
        {
            await _context.PlayerChests.AddAsync(playerChest);  // Stage new entity for insertion in the next SaveChanges call
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return playerChest;
        }

        // Performs database query and transactional persistence workflow for update player chest.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching PlayerChest entity result or default if not found.
        public async Task<PlayerChest> UpdatePlayerChest(PlayerChest playerChest)
        {
            _context.PlayerChests.Update(playerChest);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return playerChest;
        }


        // Performs database query and transactional persistence workflow for get player daily login.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching PlayerDailyLogin? entity result or default if not found.
        public async Task<PlayerDailyLogin?> GetPlayerDailyLogin(int playerProfileId)
        {
            return await _context.PlayerDailyLogins
                .FirstOrDefaultAsync(x => x.PlayerProfileId == playerProfileId);  // Fetch single matching record or null if not found
        }

        // Persists state modifications to the database for create player daily login.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching PlayerDailyLogin entity result or default if not found.
        public async Task<PlayerDailyLogin> CreatePlayerDailyLogin(PlayerDailyLogin login)
        {
            await _context.PlayerDailyLogins.AddAsync(login);  // Stage new entity for insertion in the next SaveChanges call
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return login;
        }

        // Performs database query and transactional persistence workflow for update player daily login.
        // Query details: eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync; sorts records according to business ordering rules.
        // Returns the matching PlayerDailyLogin entity result or default if not found.
        public async Task<PlayerDailyLogin> UpdatePlayerDailyLogin(PlayerDailyLogin login)
        {
            _context.PlayerDailyLogins.Update(login);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return login;
        }

        // Queries the database to retrieve get daily login reward records.
        // Query details: eagerly loads related entity navigation properties; sorts records according to business ordering rules.
        // Returns the matching DailyLoginReward? entity result or default if not found.
        public async Task<DailyLoginReward?> GetDailyLoginReward(int dayNumber, int month, int year)
        {
            return await _context.DailyLoginRewards
                .Include(r => r.RewardItem)  // Eagerly load related navigation entities to avoid N+1 queries
                .Where(r => r.DayNumber == dayNumber && r.IsActive &&  // Filter records matching the predicate
                    ((r.Month == month && r.Year == year) ||
                     (r.Month == null && r.Year == null)))
                .OrderByDescending(r => r.Month.HasValue)  // Sort results newest/highest first
                .FirstOrDefaultAsync();  // Fetch single matching record or null if not found
        }
    }
}
