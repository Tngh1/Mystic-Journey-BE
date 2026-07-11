using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Quản lý vật phẩm trong game.
    // Game APIs: Xem danh sách vật phẩm, tìm kiếm vật phẩm nhiệm vụ.
    // Admin APIs: Tạo, cập nhật vật phẩm.
    public interface IItemRepository
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy tất cả vật phẩm loại nhiệm vụ (QuestItem).
        Task<List<Item>> GetQuestItems();

        // Tìm vật phẩm nhiệm vụ theo danh sách tên.
        Task<Item?> GetQuestItemByNames(params string[] names);

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy vật phẩm theo mã định danh.
        Task<Item?> GetItemById(int id);

        // Lấy vật phẩm kèm chỉ số trang bị (stats).
        Task<Item?> GetItemByIdWithStats(int id);

        // Cập nhật thông tin vật phẩm.
        Task<Item> UpdateItem(Item item);

        // Lấy danh sách vật phẩm có phân trang, lọc theo tìm kiếm, loại, độ hiếm và trạng thái.
        Task<(int TotalCount, List<Item> Items)> GetItemsPaged(int page, int pageSize, string? search, string? type, string? rarity, bool? isActive, string? sortBy = null, string? sortOrder = null);

        // Đếm tổng số vật phẩm trong hệ thống.
        Task<int> GetTotalItemsCount();
    }
}
