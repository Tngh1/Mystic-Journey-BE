using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Quản lý shop items (vật phẩm trong cửa hàng).
    // Game APIs: Xem danh sách, xem chi tiết item.
    // Admin APIs: Tạo, cập nhật shop item.
    public interface IShopItemService
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy chi tiết shop item theo ID.
        Task<ShopItemResponseDto?> GetShopItemById(int id);

        // Lấy danh sách tất cả shop items có phân trang và lọc.
        Task<PagedResultDto<ShopItemResponseDto>> GetShopItemsPaged(int page, int pageSize, string? search, string? currency, bool? isActive);

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Tạo shop item mới.
        Task<ShopItemResponseDto> CreateShopItem(CreateShopItemRequestDto request);

        // Cập nhật shop item hiện có.
        Task<ShopItemResponseDto> UpdateShopItem(int id, UpdateShopItemRequestDto request);
    }
}
