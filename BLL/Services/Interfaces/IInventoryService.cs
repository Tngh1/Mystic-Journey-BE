using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Quản lý inventory (hành trang) của người chơi.
    // Cho phép xem, trang bị, gỡ trang bị, và sử dụng item.
    public interface IInventoryService
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Người chơi)
        // ═══════════════════════════════════════════════════════════════════════

        // Lấy tóm tắt inventory của player (số lượng items, skins, dung lượng túi).
        Task<InventorySummaryDto> GetInventory(int playerProfileId);

        // Lấy inventory đầy đủ của player với chi tiết từng item.
        Task<PlayerMeInventoryResponseDto> GetMeInventory(int playerProfileId);

        // Trang bị item từ inventory vào slot tương ứng.
        // Trả về item đã trang bị kèm stats hiện tại của player sau khi trang bị.
        Task<InventoryActionResultDto> EquipItem(int actorPlayerProfileId, EquipItemRequestDto request);

        // Gỡ item đã trang bị và trả về inventory.
        // Trả về item đã gỡ kèm stats hiện tại của player sau khi gỡ.
        Task<InventoryActionResultDto> UnequipItem(int actorPlayerProfileId, UnequipItemRequestDto request);

        // Sử dụng item có thể tiêu thụ (consumable). Giảm số lượng item trong inventory.
        Task ConsumeItem(int actorPlayerProfileId, ConsumeItemRequestDto request);

        // Thêm item vào inventory của player.
        Task<InventoryItemResponseDto> AddItemToInventory(int playerProfileId, int itemId, int quantity);

        // Trang bị skin cho nhân vật.
        Task<PlayerSkinResponseDto> EquipSkin(int actorPlayerProfileId, BLL.DTOs.EquipSkinRequestDto request);

        // Gỡ skin đang trang bị.
        Task UnequipSkin(int actorPlayerProfileId, BLL.DTOs.UnequipSkinRequestDto request);
    }
}
