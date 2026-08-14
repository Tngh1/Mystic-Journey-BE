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
    /// Triển khai các thao tác truy cập dữ liệu cho thế giới game (NPC, Rương, Đăng nhập hàng ngày) sử dụng Entity Framework.
    /// </summary>
    public class WorldRepository : IWorldRepository
    {
        private readonly MysticJourneyDbContext _context;

        public WorldRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        // ── NPC ──

        /// <summary>
        /// Lấy danh sách NPC trên bản đồ, bao gồm hội thoại đang hoạt động cùng nhiệm vụ và shop liên kết.
        /// </summary>
        public async Task<List<NPC>> GetNpcsByMapName(string mapName, int take)
        {
            return await _context.NPCs
                .Include(n => n.Dialogues.Where(d => d.IsActive))
                    .ThenInclude(d => d.LinkedQuest)
                .Include(n => n.Dialogues.Where(d => d.IsActive))
                    .ThenInclude(d => d.LinkedShopItem)
                        .ThenInclude(si => si!.Item)
                .Where(n => n.IsActive && n.MapName == mapName)
                .OrderBy(n => n.NPCId)
                .Take(take)
                .ToListAsync();
        }

        /// <summary>Lấy tất cả tên bản đồ có NPC đang hoạt động.</summary>
        public async Task<List<string>> GetAllNpcMapNames()
        {
            return await _context.NPCs
                .AsNoTracking()
                .Where(n => n.IsActive)
                .Select(n => n.MapName)
                .ToListAsync();
        }

        /// <summary>Lấy thông tin NPC theo mã, kèm hội thoại và nhiệm vụ liên kết.</summary>
        public async Task<NPC?> GetNpcById(int npcId)
        {
            return await _context.NPCs
                .Include(n => n.Dialogues.Where(d => d.IsActive))
                    .ThenInclude(d => d.LinkedQuest)
                .Include(n => n.Dialogues.Where(d => d.IsActive))
                    .ThenInclude(d => d.LinkedShopItem)
                        .ThenInclude(si => si!.Item)
                .FirstOrDefaultAsync(n => n.NPCId == npcId && n.IsActive);
        }

        /// <summary>Kiểm tra nhiệm vụ có được gán cho NPC hay không (qua bảng NPCDialogues).</summary>
        public async Task<bool> IsQuestLinkedToNpc(int npcId, int questId)
        {
            return await _context.NPCDialogues
                .AnyAsync(d => d.NPCId == npcId && d.LinkedQuestId == questId && d.IsActive);
        }

        // ── Chest ──

        /// <summary>Lấy rương kho báu theo mã, kèm vật phẩm bên trong.</summary>
        public async Task<Chest?> GetChestById(int chestId)
        {
            return await _context.Chests
                .Include(c => c.ChestItems)
                    .ThenInclude(ci => ci.Item)
                .FirstOrDefaultAsync(c => c.ChestId == chestId && c.IsActive);
        }

        /// <summary>Lấy rương của người chơi kèm chi tiết rương và vật phẩm.</summary>
        public async Task<PlayerChest?> GetPlayerChest(int playerChestId, int playerProfileId)
        {
            return await _context.PlayerChests
                .Include(pc => pc.Chest)
                    .ThenInclude(c => c!.ChestItems)
                        .ThenInclude(ci => ci.Item)
                .FirstOrDefaultAsync(pc =>
                    pc.PlayerChestId == playerChestId &&
                    pc.PlayerProfileId == playerProfileId);
        }

        /// <summary>Tạo rương mới cho người chơi.</summary>
        public async Task<PlayerChest> CreatePlayerChest(PlayerChest playerChest)
        {
            await _context.PlayerChests.AddAsync(playerChest);
            await _context.SaveChangesAsync();
            return playerChest;
        }

        /// <summary>Cập nhật trạng thái rương của người chơi.</summary>
        public async Task<PlayerChest> UpdatePlayerChest(PlayerChest playerChest)
        {
            _context.PlayerChests.Update(playerChest);
            await _context.SaveChangesAsync();
            return playerChest;
        }

        // ── Daily Login ──

        /// <summary>Lấy thông tin đăng nhập hàng ngày của người chơi.</summary>
        public async Task<PlayerDailyLogin?> GetPlayerDailyLogin(int playerProfileId)
        {
            return await _context.PlayerDailyLogins
                .FirstOrDefaultAsync(x => x.PlayerProfileId == playerProfileId);
        }

        /// <summary>Tạo bản ghi đăng nhập hàng ngày cho người chơi.</summary>
        public async Task<PlayerDailyLogin> CreatePlayerDailyLogin(PlayerDailyLogin login)
        {
            await _context.PlayerDailyLogins.AddAsync(login);
            await _context.SaveChangesAsync();
            return login;
        }

        /// <summary>Cập nhật thông tin đăng nhập hàng ngày.</summary>
        public async Task<PlayerDailyLogin> UpdatePlayerDailyLogin(PlayerDailyLogin login)
        {
            _context.PlayerDailyLogins.Update(login);
            await _context.SaveChangesAsync();
            return login;
        }

        /// <summary>Lấy phần thưởng đăng nhập theo số ngày, kèm vật phẩm thưởng.</summary>
        public async Task<DailyLoginReward?> GetDailyLoginReward(int dayNumber, int month, int year)
        {
            return await _context.DailyLoginRewards
                .Include(r => r.RewardItem)
                .Where(r => r.DayNumber == dayNumber && r.IsActive &&
                    ((r.Month == month && r.Year == year) ||
                     (r.Month == null && r.Year == null)))
                .OrderByDescending(r => r.Month.HasValue)
                .FirstOrDefaultAsync();
        }
    }
}
