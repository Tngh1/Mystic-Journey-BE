using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class PlayerProfileRepository : IPlayerProfileRepository
    {
        private readonly MysticJourneyDbContext _context;

        public PlayerProfileRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<PlayerProfile?> GetPlayerProfileById(int id)
        {
            return await _context.PlayerProfiles
                .FirstOrDefaultAsync(p => p.PlayerProfileId == id);
        }

        public async Task<PlayerProfile?> GetPlayerProfileByIdWithStats(int id)
        {
            return await _context.PlayerProfiles
                .Include(p => p.PlayerStats)
                .FirstOrDefaultAsync(p => p.PlayerProfileId == id);
        }

        public async Task<PlayerProfile?> GetByIdFull(int id)
        {
            return await _context.PlayerProfiles
                .Include(p => p.PlayerStats)
                .Include(p => p.Account)
                .FirstOrDefaultAsync(p => p.PlayerProfileId == id);
        }

        public async Task<PlayerProfile?> GetByIdWithAll(int id)
        {
            return await _context.PlayerProfiles
                .Include(p => p.PlayerStats)
                .Include(p => p.Account)
                .Include(p => p.InventoryItems).ThenInclude(i => i.Item)
                .Include(p => p.PlayerSkills).ThenInclude(ps => ps.Skill)
                .Include(p => p.PlayerQuests).ThenInclude(pq => pq.Quest).ThenInclude(q => q.RewardItem)
                .Include(p => p.Mails).ThenInclude(m => m.AttachedItem)
                .Include(p => p.PlayerAchievements).ThenInclude(pa => pa.Achievement).ThenInclude(a => a.RewardItem)
                .FirstOrDefaultAsync(p => p.PlayerProfileId == id);
        }

        public async Task<PlayerProfile?> GetByAccountId(int accountId)
        {
            return await _context.PlayerProfiles
                .Include(p => p.PlayerStats)
                .Include(p => p.Account)
                .FirstOrDefaultAsync(p => p.AccountId == accountId);
        }

        public async Task<PlayerStatsSnapshot?> GetSnapshotByPlayerProfileId(int playerProfileId)
        {
            return await _context.PlayerStatsSnapshots.FirstOrDefaultAsync(s => s.PlayerProfileId == playerProfileId);
        }

        public async Task<List<PlayerProfile>> GetAllPlayerProfiles()
        {
            return await _context.PlayerProfiles.ToListAsync();
        }

        public async Task<PlayerProfile> CreatePlayerProfile(PlayerProfile profile)
        {
            profile.CreatedAt = DateTime.UtcNow;
            await _context.PlayerProfiles.AddAsync(profile);
            await _context.SaveChangesAsync();
            return profile;
        }

        public async Task<PlayerProfile> UpdatePlayerProfile(PlayerProfile profile)
        {
            profile.UpdatedAt = DateTime.UtcNow;
            _context.PlayerProfiles.Update(profile);
            await _context.SaveChangesAsync();
            return profile;
        }

        public async Task<List<PlayerProfile>> Search(string? keyword = null, string? playerClass = null)
        {
            var query = _context.PlayerProfiles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var lowerKeyword = keyword.ToLower();
                query = query.Where(p =>
                    p.DisplayName.ToLower().Contains(lowerKeyword) ||
                    (p.Account != null && p.Account.UserName.ToLower().Contains(lowerKeyword)));
            }

            if (!string.IsNullOrWhiteSpace(playerClass))
            {
                query = query.Where(p => p.Class == playerClass);
            }

            return await query.ToListAsync();
        }

        public async Task<int> GetTotalPlayerProfilesCount()
        {
            return await _context.PlayerProfiles.CountAsync();
        }

        public async Task<(int TotalCount, List<PlayerProfile> Items)> GetProfilesPaged(int page, int pageSize, string? search, int? level)
        {
            var query = _context.PlayerProfiles
                .Include(p => p.Account)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.DisplayName.Contains(search));
            }
            if (level.HasValue)
            {
                query = query.Where(x => x.Level == level.Value);
            }

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (totalCount, items);
        }

        public async Task<List<PlayerProfile>> GetFriends(int playerProfileId)
        {
            return await _context.Friends
                .Where(f => (f.RequesterId == playerProfileId || f.AddresseeId == playerProfileId) && f.Status == "Accepted")
                .Include(f => f.Requester)
                .Include(f => f.Addressee)
                .Select(f => f.RequesterId == playerProfileId ? f.Addressee! : f.Requester!)
                .Where(p => p != null)
                .ToListAsync();
        }
    }
}
