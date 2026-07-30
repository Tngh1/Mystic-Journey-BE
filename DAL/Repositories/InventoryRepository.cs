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
    /// <summary>
    /// Triển khai các thao tác truy cập dữ liệu cho túi đồ và skin người chơi sử dụng Entity Framework.
    /// </summary>
    public class InventoryRepository : IInventoryRepository
    {
        private readonly MysticJourneyDbContext _context;

        public InventoryRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        // ── Inventory ──

        /// <summary>Tìm vật phẩm trong túi đồ theo mã, kèm thông tin vật phẩm và người sở hữu.</summary>
        public async Task<InventoryItem?> GetById(int id)
        {
            return await _context.InventoryItems
                .Include(i => i.Item)
                    .ThenInclude(it => it!.EquipmentStats)
                .Include(i => i.PlayerProfile)
                .FirstOrDefaultAsync(i => i.InventoryItemId == id);
        }

        /// <summary>Tìm vật phẩm trong túi của người chơi theo mã vật phẩm.</summary>
        public async Task<InventoryItem?> GetByPlayerAndItem(int playerProfileId, int itemId)
        {
            return await _context.InventoryItems
                .Include(i => i.Item)
                    .ThenInclude(it => it!.EquipmentStats)
                .FirstOrDefaultAsync(i => i.PlayerProfileId == playerProfileId && i.ItemId == itemId);
        }

        /// <summary>Lấy toàn bộ túi đồ của người chơi.</summary>
        // ThenInclude(EquipmentStats): chỉ số trang bị nằm ở bảng riêng. Không eager-load thì
        // AutoMapper thấy null và trả 0 cho mọi chỉ số → popup chi tiết vật phẩm trống trơn.
        public async Task<List<InventoryItem>> GetByPlayerId(int playerProfileId)
        {
            return await _context.InventoryItems
                .Include(i => i.Item)
                    .ThenInclude(it => it!.EquipmentStats)
                .Where(i => i.PlayerProfileId == playerProfileId)
                .ToListAsync();
        }

        /// <summary>Thêm vật phẩm vào túi đồ (tự động ghi nhận thời gian tạo).</summary>
        public async Task<InventoryItem> AddItem(InventoryItem item)
        {
            item.CreatedAt = DateTime.UtcNow;
            await _context.InventoryItems.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        /// <summary>Cập nhật vật phẩm trong túi đồ.</summary>
        public async Task<InventoryItem> UpdateItem(InventoryItem item)
        {
            _context.InventoryItems.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }

        /// <summary>Xóa vật phẩm khỏi túi đồ (xóa vĩnh viễn).</summary>
        public async Task DeleteItem(int id)
        {
            var item = await _context.InventoryItems.FindAsync(id);
            if (item != null)
            {
                _context.InventoryItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        // ── Skin ──

        /// <summary>Tìm skin của người chơi theo mã, kèm thông tin skin.</summary>
        public async Task<PlayerSkin?> GetPlayerSkinById(int id)
        {
            return await _context.PlayerSkins
                .Include(ps => ps.Skin)
                .FirstOrDefaultAsync(ps => ps.PlayerSkinId == id);
        }

        /// <summary>Lấy tất cả skin của một người chơi.</summary>
        public async Task<List<PlayerSkin>> GetPlayerSkinsByPlayerId(int playerProfileId)
        {
            return await _context.PlayerSkins
                .Include(ps => ps.Skin)
                .Where(ps => ps.PlayerProfileId == playerProfileId)
                .ToListAsync();
        }

        /// <summary>Cập nhật skin của người chơi.</summary>
        public async Task<PlayerSkin> UpdatePlayerSkin(PlayerSkin skin)
        {
            _context.PlayerSkins.Update(skin);
            await _context.SaveChangesAsync();
            return skin;
        }

        /// <summary>Lấy tất cả skin đang hoạt động trong hệ thống.</summary>
        public async Task<List<Skin>> GetAllActiveSkins()
        {
            return await _context.Skins.Where(s => s.IsActive).ToListAsync();
        }
    }
}
