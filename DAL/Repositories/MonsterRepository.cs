using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    /// <summary>
    /// Triển khai các thao tác truy cập dữ liệu cho quái vật và vật phẩm rơi sử dụng Entity Framework.
    /// </summary>
    public class MonsterRepository : IMonsterRepository
    {
        private readonly MysticJourneyDbContext _context;

        public MonsterRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        // ── Thống kê ──

        /// <summary>Đếm tổng số quái vật trong hệ thống.</summary>
        public async Task<int> GetTotalMonstersCount()
        {
            return await _context.Monsters.CountAsync();
        }

        // ── Query ──

        /// <summary>Tìm quái vật theo mã định danh.</summary>
        public async Task<Monster?> GetMonsterById(int id)
        {
            return await _context.Monsters
                .FirstOrDefaultAsync(m => m.MonsterId == id);
        }

        /// <summary>Lấy quái vật kèm danh sách vật phẩm rơi.</summary>
        public async Task<Monster?> GetMonsterByIdWithDrops(int id)
        {
            return await _context.Monsters
                .Include(m => m.MonsterDrops)
                    .ThenInclude(d => d.Item)
                .FirstOrDefaultAsync(m => m.MonsterId == id);
        }

        /// <summary>Lấy thông tin khám phá quái vật của người chơi.</summary>
        public async Task<PlayerMonsterDiscovery?> GetPlayerDiscovery(int playerProfileId, int monsterId)
        {
            return await _context.PlayerMonsterDiscoveries
                .FirstOrDefaultAsync(d => d.PlayerProfileId == playerProfileId && d.MonsterId == monsterId);
        }

        /// <summary>Lấy dictionary khám phá quái vật của người chơi (key: monsterId).</summary>
        public async Task<Dictionary<int, PlayerMonsterDiscovery>> GetPlayerDiscoveriesDict(int playerProfileId)
        {
            return await _context.PlayerMonsterDiscoveries
                .AsNoTracking()
                .Where(d => d.PlayerProfileId == playerProfileId)
                .ToDictionaryAsync(d => d.MonsterId);
        }

        /// <summary>
        /// Lấy danh sách mã quái vật boss thuộc nhiệm vụ đã hoàn thành.
        /// So sánh các nhiệm vụ có boss với danh sách nhiệm vụ đã hoàn thành của người chơi.
        /// </summary>
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

        /// <summary>
        /// Lấy các điểm spawn đang hoạt động.
        /// Nếu có dungeonId thì lọc theo dungeon, ngược lại lọc theo bản đồ và vùng (region).
        /// </summary>
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

        /// <summary>Lấy danh sách mã quái vật đã được khám phá bởi người chơi.</summary>
        public async Task<HashSet<int>> GetDiscoveredMonsterIds(int playerProfileId)
        {
            var ids = await _context.PlayerMonsterDiscoveries
                .AsNoTracking()
                .Where(d => d.PlayerProfileId == playerProfileId && d.IsDiscovered)
                .Select(d => d.MonsterId)
                .ToListAsync();
            return ids.ToHashSet();
        }

        /// <summary>Lấy các vật phẩm rơi đang hoạt động của quái vật.</summary>
        public async Task<List<MonsterDrop>> GetActiveDropsByMonsterId(int monsterId)
        {
            return await _context.MonsterDrops
                .Include(d => d.Item)
                .Where(d => d.MonsterId == monsterId && d.IsActive)
                .ToListAsync();
        }

        /// <summary>Lấy tất cả vật phẩm rơi của quái vật (kể cả không hoạt động).</summary>
        public async Task<List<MonsterDrop>> GetDropsByMonsterId(int monsterId)
        {
            return await _context.MonsterDrops
                .Include(d => d.Item)
                .Where(d => d.MonsterId == monsterId)
                .ToListAsync();
        }

        /// <summary>Lấy các điểm spawn của quái vật theo mã.</summary>
        public async Task<List<MonsterSpawn>> GetSpawnsByMonsterId(int monsterId)
        {
            return await _context.MonsterSpawns
                .Where(s => s.MonsterId == monsterId && s.IsActive)
                .ToListAsync();
        }

        // ── CRUD ──

        /// <summary>Cập nhật thông tin quái vật.</summary>
        public async Task<Monster> UpdateMonster(Monster monster)
        {
            _context.Monsters.Update(monster);
            await _context.SaveChangesAsync();
            return monster;
        }

        /// <summary>Thêm vật phẩm rơi cho quái vật.</summary>
        public async Task<MonsterDrop> CreateDrop(MonsterDrop drop)
        {
            await _context.MonsterDrops.AddAsync(drop);
            await _context.SaveChangesAsync();
            return drop;
        }

        /// <summary>
        /// Tạo hoặc cập nhật trạng thái khám phá quái vật.
        /// Nếu đã tồn tại thì cập nhật, ngược lại tạo mới.
        /// </summary>
        public async Task<PlayerMonsterDiscovery> CreateOrUpdatePlayerDiscovery(PlayerMonsterDiscovery discovery)
        {
            var existing = await _context.PlayerMonsterDiscoveries
                .FirstOrDefaultAsync(d => d.PlayerProfileId == discovery.PlayerProfileId && d.MonsterId == discovery.MonsterId);

            if (existing != null)
            {
                existing.IsDiscovered = discovery.IsDiscovered;
                existing.DiscoveredAt = discovery.DiscoveredAt;
                existing.TimesDefeated = discovery.TimesDefeated;
                _context.PlayerMonsterDiscoveries.Update(existing);
            }
            else
            {
                await _context.PlayerMonsterDiscoveries.AddAsync(discovery);
            }
            await _context.SaveChangesAsync();
            return discovery;
        }

        /// <summary>Tạo điểm spawn mới cho quái vật.</summary>
        public async Task<MonsterSpawn> CreateSpawn(MonsterSpawn spawn)
        {
            await _context.MonsterSpawns.AddAsync(spawn);
            await _context.SaveChangesAsync();
            return spawn;
        }

        // ── Phân trang ──

        /// <summary>Lấy danh sách quái vật có phân trang, lọc theo tìm kiếm (tên), loại và trạng thái hoạt động.</summary>
        public async Task<(int TotalCount, List<Monster> Items)> GetMonstersPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? sortBy = null, string? sortOrder = null)
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

            bool desc = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);
            query = (sortBy?.ToLowerInvariant()) switch
            {
                "name" => desc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
                "type" => desc ? query.OrderByDescending(x => x.Type) : query.OrderBy(x => x.Type),
                "level" => desc ? query.OrderByDescending(x => x.Level) : query.OrderBy(x => x.Level),
                "maxhp" => desc ? query.OrderByDescending(x => x.MaxHp) : query.OrderBy(x => x.MaxHp),
                "attack" => desc ? query.OrderByDescending(x => x.Atk) : query.OrderBy(x => x.Atk),
                "defense" => desc ? query.OrderByDescending(x => x.Def) : query.OrderBy(x => x.Def),
                "goldreward" => desc ? query.OrderByDescending(x => x.GoldReward) : query.OrderBy(x => x.GoldReward),
                "expReward" => desc ? query.OrderByDescending(x => x.ExperienceReward) : query.OrderBy(x => x.ExperienceReward),
                "isactive" => desc ? query.OrderByDescending(x => x.IsActive) : query.OrderBy(x => x.IsActive),
                _ => desc ? query.OrderByDescending(x => x.MonsterId) : query.OrderBy(x => x.MonsterId),
            };

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (totalCount, items);
        }

        /// <summary>Lấy danh sách vật phẩm rơi đang hoạt động có phân trang.</summary>
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
