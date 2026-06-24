using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class MonsterRepository : IMonsterRepository
    {
        private readonly MysticJourneyDbContext _context;

        public MonsterRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<Monster?> GetMonsterById(int id)
        {
            return await _context.Monsters
                .FirstOrDefaultAsync(m => m.MonsterId == id);
        }

        public async Task<Monster?> GetMonsterByIdWithDrops(int id)
        {
            return await _context.Monsters
                .Include(m => m.MonsterDrops)
                    .ThenInclude(d => d.Item)
                .FirstOrDefaultAsync(m => m.MonsterId == id);
        }

        public async Task<Monster> CreateMonster(Monster monster)
        {
            await _context.Monsters.AddAsync(monster);
            await _context.SaveChangesAsync();
            return monster;
        }

        public async Task<Monster> UpdateMonster(Monster monster)
        {
_context.Monsters.Update(monster);
            await _context.SaveChangesAsync();
            return monster;
        }


        public async Task<MonsterDrop> CreateDrop(MonsterDrop drop)
        {
            await _context.MonsterDrops.AddAsync(drop);
            await _context.SaveChangesAsync();
            return drop;
        }

        public async Task<PlayerMonsterDiscovery?> GetPlayerDiscovery(int playerProfileId, int monsterId)
        {
            return await _context.PlayerMonsterDiscoveries
                .FirstOrDefaultAsync(d => d.PlayerProfileId == playerProfileId && d.MonsterId == monsterId);
        }

        public async Task<PlayerMonsterDiscovery> CreateOrUpdatePlayerDiscovery(PlayerMonsterDiscovery discovery)
        {
            var existing = await GetPlayerDiscovery(discovery.PlayerProfileId, discovery.MonsterId);
            if (existing == null)
            {
                await _context.AddAsync(discovery);
            }
            else
            {
                existing.IsDiscovered = discovery.IsDiscovered || existing.IsDiscovered;
                existing.TimesDefeated = discovery.TimesDefeated > 0
                    ? discovery.TimesDefeated
                    : existing.TimesDefeated;
                if (discovery.DiscoveredAt.HasValue)
                    existing.DiscoveredAt = discovery.DiscoveredAt;
            }
            await _context.SaveChangesAsync();
            return existing ?? discovery;
        }

        public async Task<HashSet<int>> GetCompletedQuestBossMonsterIds(int playerProfileId)
        {
            var bossIds = await _context.Quests
                .AsNoTracking()
                .Where(q => q.BossMonsterId.HasValue && q.IsActive)
                .Select(q => new { q.QuestId, q.BossMonsterId })
                .ToListAsync();

            if (bossIds.Count == 0)
                return new HashSet<int>();

            var completedQuestIds = await _context.PlayerQuests
                .AsNoTracking()
                .Where(pq => pq.PlayerProfileId == playerProfileId &&
                             (pq.Status == "Completed" || pq.Status == "Claimed"))
                .Select(pq => pq.QuestId)
                .ToListAsync();

            return bossIds
                .Where(q => completedQuestIds.Contains(q.QuestId))
                .Select(q => q.BossMonsterId!.Value)
                .ToHashSet();
        }

        public async Task<List<MonsterSpawn>> GetActiveSpawns(string mapName, string? regionName, int? dungeonId)
        {
            var query = _context.MonsterSpawns
                .Include(s => s.Monster)
                .Include(s => s.Dungeon)
                .Where(s => s.IsActive && s.Monster != null && s.Monster.IsActive);

            if (dungeonId.HasValue)
            {
                query = query.Where(s => s.DungeonId == dungeonId.Value);
            }
            else
            {
                query = query.Where(s => s.MapName == mapName && s.DungeonId == null);
                if (!string.IsNullOrWhiteSpace(regionName))
                    query = query.Where(s => s.RegionName == regionName);
            }

            return await query
                .OrderBy(s => s.MonsterSpawnId)
                .ToListAsync();
        }

        public async Task<HashSet<int>> GetDiscoveredMonsterIds(int playerProfileId)
        {
            return await _context.PlayerMonsterDiscoveries
                .AsNoTracking()
                .Where(d => d.PlayerProfileId == playerProfileId && d.IsDiscovered)
                .Select(d => d.MonsterId)
                .ToHashSetAsync();
        }

        public async Task<List<MonsterDrop>> GetActiveDropsByMonsterId(int monsterId)
        {
            return await _context.MonsterDrops
                .Include(d => d.Item)
                .Where(d => d.MonsterId == monsterId && d.IsActive)
                .ToListAsync();
        }

        public async Task<List<MonsterSpawn>> GetSpawnsByMonsterId(int monsterId)
        {
            return await _context.MonsterSpawns
                .Where(s => s.MonsterId == monsterId && s.IsActive)
                .ToListAsync();
        }

        public async Task<MonsterSpawn> CreateSpawn(MonsterSpawn spawn)
        {
            await _context.MonsterSpawns.AddAsync(spawn);
            await _context.SaveChangesAsync();
            return spawn;
        }

        public async Task<List<MonsterDrop>> GetDropsByMonsterId(int monsterId)
        {
            return await _context.MonsterDrops
                .Include(d => d.Item)
                .Where(d => d.MonsterId == monsterId)
                .ToListAsync();
        }

        public async Task<(int TotalCount, List<Monster> Items)> GetMonstersPaged(int page, int pageSize, string? search, string? type, bool? isActive)
        {
            var query = _context.Monsters
                .Include(m => m.MonsterDrops)
                    .ThenInclude(d => d.Item)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.Contains(search));
            }
            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(x => x.Type == type);
            }
            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);
            }

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (totalCount, items);
        }

        public async Task<(int TotalCount, List<MonsterDrop> Items)> GetMonsterDropsPaged(int page, int pageSize)
        {
            var query = _context.MonsterDrops
                .Include(d => d.Item)
                .Where(d => d.IsActive)
                .AsNoTracking();

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (totalCount, items);
        }
    }
}
