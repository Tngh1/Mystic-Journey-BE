using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Quản lý vật phẩm trong cửa hàng.
    // Game APIs: Xem danh sách vật phẩm.
    // Admin APIs: Tạo, cập nhật vật phẩm.
    public interface IShopItemRepository
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy vật phẩm shop theo mã định danh.
        Task<ShopItem?> GetShopItemById(int id);

        // Lấy vật phẩm shop kèm thông tin vật phẩm.
        Task<ShopItem?> GetShopItemByIdWithItem(int id);

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Tạo vật phẩm shop mới.
        Task<ShopItem> CreateShopItem(ShopItem shopItem);

        // Cập nhật vật phẩm shop.
        Task<ShopItem> UpdateShopItem(ShopItem shopItem);

        // Lấy danh sách vật phẩm shop có phân trang, lọc theo tìm kiếm, loại tiền và trạng thái.
        Task<(int TotalCount, List<ShopItem> Items)> GetShopItemsPaged(int page, int pageSize, string? search, string? currency, bool? isActive, string? sortBy = null, string? sortOrder = null);
    }
}
