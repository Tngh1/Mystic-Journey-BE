using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Quản lý items (vật phẩm) trong game.
    // Game APIs: Xem danh sách, xem chi tiết item.
    // Admin APIs: Tạo, cập nhật item.
    public interface IItemService
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy chi tiết item theo ID.
        Task<ItemResponseDto?> GetItemById(int id);

        // Lấy danh sách tất cả items có phân trang và lọc.
        Task<PagedResultDto<ItemResponseDto>> GetItemsPaged(int page, int pageSize, string? search, string? type, string? rarity, bool? isActive);

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // Tạo item mới.
        Task<ItemResponseDto> CreateItem(CreateItemRequestDto request);

        // Cập nhật item hiện có.
        Task<ItemResponseDto> UpdateItem(int id, UpdateItemRequestDto request);
    }
}
