using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    // Queries the database to retrieve i monster repository records.
    public class MonsterRepository : IMonsterRepository
    {
        private readonly MysticJourneyDbContext _context;

        // Initializes a new instance of MonsterRepository with dependencies: context.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public MonsterRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }


        // Queries the database to retrieve get total monsters count records.
        // Returns the computed numeric count or database ID result.
        public async Task<int> GetTotalMonstersCount()
        {
            return await _context.Monsters.CountAsync();
        }


        // Queries the database to retrieve get monster by id records.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties.
        // Returns the matching Monster? entity result or default if not found.
        public async Task<Monster?> GetMonsterById(int id)
        {
            return await _context.Monsters
                .FirstOrDefaultAsync(m => m.MonsterId == id);  // Fetch single matching record or null if not found
        }

        // Queries the database to retrieve get monster by id with drops records.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties.
        // Returns the matching Monster? entity result or default if not found.
        public async Task<Monster?> GetMonsterByIdWithDrops(int id)
        {
            return await _context.Monsters
                .Include(m => m.MonsterDrops)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(d => d.Item)
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking()  // Disable EF Core change tracking for this read-only query
                .FirstOrDefaultAsync(m => m.MonsterId == id);  // Fetch single matching record or null if not found
        }

        // Queries the database to retrieve get player discovery records.
        // Query details: uses AsNoTracking() for read-only query optimization.
        // Returns the matching PlayerMonsterDiscovery? entity result or default if not found.
        public async Task<PlayerMonsterDiscovery?> GetPlayerDiscovery(int playerProfileId, int monsterId)
        {
            return await _context.PlayerMonsterDiscoveries
                .FirstOrDefaultAsync(d => d.PlayerProfileId == playerProfileId && d.MonsterId == monsterId);  // Fetch single matching record or null if not found
        }

        // Queries the database to retrieve get player discoveries dict records.
        // Query details: uses AsNoTracking() for read-only query optimization.
        // Returns the matching Dictionary<int, PlayerMonsterDiscovery entity result or default if not found.
        public async Task<Dictionary<int, PlayerMonsterDiscovery>> GetPlayerDiscoveriesDict(int playerProfileId)
        {
            return await _context.PlayerMonsterDiscoveries
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking()  // Disable EF Core change tracking for this read-only query
                .Where(d => d.PlayerProfileId == playerProfileId)  // Filter records matching the predicate
                .ToDictionaryAsync(d => d.MonsterId);
        }

        // Queries the database to retrieve get completed quest boss monster ids records.
        // Query details: uses AsNoTracking() for read-only query optimization.
        // Returns the matching HashSet<int entity result or default if not found.
        public async Task<HashSet<int>> GetCompletedQuestBossMonsterIds(int playerProfileId)
        {
            var bossIds = await _context.Quests
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking()  // Disable EF Core change tracking for this read-only query
                .Where(q => q.BossMonsterId.HasValue && q.IsActive)  // Filter records matching the predicate
                .Select(q => new { q.QuestId, q.BossMonsterId })
                .ToListAsync();  // Materialize the query into a list from the database

            if (bossIds.Count == 0)
                return new HashSet<int>();

            var completedQuestIds = await _context.PlayerQuests
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking()  // Disable EF Core change tracking for this read-only query
                .Where(pq => pq.PlayerProfileId == playerProfileId &&  // Filter records matching the predicate
                             (pq.Status == "Completed" || pq.Status == "Claimed"))
                .Select(pq => pq.QuestId)
                .ToListAsync();  // Materialize the query into a list from the database

            return bossIds
                .Where(q => completedQuestIds.Contains(q.QuestId))  // Filter records matching the predicate
                .Select(q => q.BossMonsterId!.Value)
                .ToHashSet();
        }

        // Queries the database to retrieve get active spawns records.
        // Query details: eagerly loads related entity navigation properties.
        // Returns the matching List<MonsterSpawn entity result or default if not found.
        public async Task<List<MonsterSpawn>> GetActiveSpawns(string mapName, string? regionName, int? dungeonId)
        {
            var query = _context.MonsterSpawns
                .Include(s => s.Monster)  // Eagerly load related navigation entities to avoid N+1 queries
                .Include(s => s.Dungeon)  // Eagerly load related navigation entities to avoid N+1 queries
                .Where(s => s.IsActive && s.Monster != null && s.Monster.IsActive);  // Filter records matching the predicate

            if (dungeonId.HasValue)
            {
                query = query.Where(s => s.DungeonId == dungeonId.Value);  // Filter records matching the predicate
            }
            else
            {
                query = query.Where(s => s.MapName == mapName && s.DungeonId == null);  // Filter records matching the predicate
                if (!string.IsNullOrWhiteSpace(regionName))
                    query = query.Where(s => s.RegionName == regionName);  // Filter records matching the predicate
            }

            return await query
                .OrderBy(s => s.MonsterSpawnId)  // Sort results oldest/lowest first
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking()  // Disable EF Core change tracking for this read-only query
                .ToListAsync();  // Materialize the query into a list from the database
        }

        // Queries the database to retrieve get discovered monster ids records.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties.
        // Returns the matching HashSet<int entity result or default if not found.
        public async Task<HashSet<int>> GetDiscoveredMonsterIds(int playerProfileId)
        {
            var ids = await _context.PlayerMonsterDiscoveries
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking()  // Disable EF Core change tracking for this read-only query
                .Where(d => d.PlayerProfileId == playerProfileId && d.IsDiscovered)  // Filter records matching the predicate
                .Select(d => d.MonsterId)
                .ToListAsync();  // Materialize the query into a list from the database
            return ids.ToHashSet();
        }

        // Queries the database to retrieve get drops by monster id records.
        // Query details: eagerly loads related entity navigation properties.
        // Returns the matching List<MonsterDrop entity result or default if not found.
        public async Task<List<MonsterDrop>> GetDropsByMonsterId(int monsterId)
        {
            return await _context.MonsterDrops
                .Include(d => d.Item)  // Eagerly load related navigation entities to avoid N+1 queries
                .Where(d => d.MonsterId == monsterId)  // Filter records matching the predicate
                .ToListAsync();  // Materialize the query into a list from the database
        }

        // Performs database query and transactional persistence workflow for get spawns by monster id.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching List<MonsterSpawn entity result or default if not found.
        public async Task<List<MonsterSpawn>> GetSpawnsByMonsterId(int monsterId)
        {
            return await _context.MonsterSpawns
                .Where(s => s.MonsterId == monsterId && s.IsActive)  // Filter records matching the predicate
                .ToListAsync();  // Materialize the query into a list from the database
        }


        // Persists state modifications to the database for update monster.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching Monster entity result or default if not found.
        public async Task<Monster> UpdateMonster(Monster monster)
        {
            _context.Monsters.Update(monster);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return monster;
        }

        // Performs database query and transactional persistence workflow for create drop.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching MonsterDrop entity result or default if not found.
        public async Task<MonsterDrop> CreateDrop(MonsterDrop drop)
        {
            await _context.MonsterDrops.AddAsync(drop);  // Stage new entity for insertion in the next SaveChanges call
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return drop;
        }

        // Performs database query and transactional persistence workflow for create or update player discovery.
        // Returns the matching PlayerMonsterDiscovery entity result or default if not found.
        public async Task<PlayerMonsterDiscovery> CreateOrUpdatePlayerDiscovery(PlayerMonsterDiscovery discovery)
        {
            var existing = await _context.PlayerMonsterDiscoveries
                .FirstOrDefaultAsync(d => d.PlayerProfileId == discovery.PlayerProfileId && d.MonsterId == discovery.MonsterId);  // Fetch single matching record or null if not found

            if (existing != null)  // Entity exists — proceed with conditional branch
            {
                existing.IsDiscovered = discovery.IsDiscovered;
                existing.DiscoveredAt = discovery.DiscoveredAt;
                existing.TimesDefeated = discovery.TimesDefeated;
                _context.PlayerMonsterDiscoveries.Update(existing);
            }
            else
            {
                await _context.PlayerMonsterDiscoveries.AddAsync(discovery);  // Stage new entity for insertion in the next SaveChanges call
            }
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return discovery;
        }

        // Performs database query and transactional persistence workflow for create spawn.
        // Query details: eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching MonsterSpawn entity result or default if not found.
        public async Task<MonsterSpawn> CreateSpawn(MonsterSpawn spawn)
        {
            await _context.MonsterSpawns.AddAsync(spawn);  // Stage new entity for insertion in the next SaveChanges call
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return spawn;
        }

        // Performs database query and transactional persistence workflow for get spawn by id.
        // Query details: eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching MonsterSpawn? entity result or default if not found.
        public async Task<MonsterSpawn?> GetSpawnById(int spawnId)
        {
            return await _context.MonsterSpawns
                .Include(s => s.Monster)  // Eagerly load related navigation entities to avoid N+1 queries
                .Include(s => s.Dungeon)  // Eagerly load related navigation entities to avoid N+1 queries
                .FirstOrDefaultAsync(s => s.MonsterSpawnId == spawnId);  // Fetch single matching record or null if not found
        }

        // Persists state modifications to the database for update spawn.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching MonsterSpawn entity result or default if not found.
        public async Task<MonsterSpawn> UpdateSpawn(MonsterSpawn spawn)
        {
            _context.MonsterSpawns.Update(spawn);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return spawn;
        }

        // Persists state modifications to the database for delete spawn.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        public async Task DeleteSpawn(int spawnId)
        {
            var spawn = await _context.MonsterSpawns.FindAsync(spawnId);
            if (spawn != null)  // Entity exists — proceed with conditional branch
            {
                _context.MonsterSpawns.Remove(spawn);  // Mark entity for deletion in the next SaveChanges call
                await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            }
        }


        // Queries the database to retrieve get monsters paged records.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties; sorts records according to business ordering rules; applies pagination offset and limit parameters.
        public async Task<(int TotalCount, List<Monster> Items)> GetMonstersPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? sortBy = null, string? sortOrder = null)
        {
            // Execute this query without change tracking because the returned entities are read-only.
            var filtered = _context.Monsters.AsNoTracking();  // Disable EF Core change tracking for this read-only query

            if (!string.IsNullOrEmpty(search))
            {
                filtered = filtered.Where(x => x.Name.Contains(search));  // Filter records matching the predicate
            }
            if (!string.IsNullOrEmpty(type))
            {
                filtered = filtered.Where(x => x.Type == type);  // Filter records matching the predicate
            }
            if (isActive.HasValue)
            {
                filtered = filtered.Where(x => x.IsActive == isActive.Value);  // Filter records matching the predicate
            }

            int totalCount = await filtered.CountAsync();

            var query = filtered
                .Include(m => m.MonsterDrops)  // Eagerly load related navigation entities to avoid N+1 queries
                    .ThenInclude(d => d.Item);

            bool desc = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);
            IQueryable<Monster> ordered = (sortBy?.ToLowerInvariant()) switch
            {
                "name" => desc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),  // Sort results newest/highest first
                "type" => desc ? query.OrderByDescending(x => x.Type) : query.OrderBy(x => x.Type),  // Sort results newest/highest first
                "level" => desc ? query.OrderByDescending(x => x.Level) : query.OrderBy(x => x.Level),  // Sort results newest/highest first
                "maxhp" => desc ? query.OrderByDescending(x => x.MaxHp) : query.OrderBy(x => x.MaxHp),  // Sort results newest/highest first
                "attack" => desc ? query.OrderByDescending(x => x.Atk) : query.OrderBy(x => x.Atk),  // Sort results newest/highest first
                "defense" => desc ? query.OrderByDescending(x => x.Def) : query.OrderBy(x => x.Def),  // Sort results newest/highest first
                "goldreward" => desc ? query.OrderByDescending(x => x.GoldReward) : query.OrderBy(x => x.GoldReward),  // Sort results newest/highest first
                "expreward" => desc ? query.OrderByDescending(x => x.ExperienceReward) : query.OrderBy(x => x.ExperienceReward),  // Sort results newest/highest first
                "isactive" => desc ? query.OrderByDescending(x => x.IsActive) : query.OrderBy(x => x.IsActive),  // Sort results newest/highest first
                _ => desc ? query.OrderByDescending(x => x.MonsterId) : query.OrderBy(x => x.MonsterId),  // Sort results newest/highest first
            };

            var items = await ordered.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();  // Materialize the query into a list from the database

            return (totalCount, items);
        }

        // Queries the database to retrieve get monster drops paged records.
        // Query details: uses AsNoTracking() for read-only query optimization.
        public async Task<(int TotalCount, List<MonsterDrop> Items)> GetMonsterDropsPaged(int page, int pageSize)
        {
            var filtered = _context.MonsterDrops
                .Where(d => d.IsActive)  // Filter records matching the predicate
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking();  // Disable EF Core change tracking for this read-only query

            int totalCount = await filtered.CountAsync();
            var items = await filtered
                .Include(d => d.Item)  // Eagerly load related navigation entities to avoid N+1 queries
                .OrderBy(d => d.MonsterDropId)  // Sort results oldest/lowest first
                .Skip((page - 1) * pageSize)  // Apply pagination offset — skip already-seen records
                .Take(pageSize)  // Apply pagination limit — cap result set size
                .ToListAsync();  // Materialize the query into a list from the database

            return (totalCount, items);
        }
    }
}
