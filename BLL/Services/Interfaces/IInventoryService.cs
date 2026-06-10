using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<InventoryItemResponseDto> AddItemToInventory(int playerProfileId, int itemId, int quantity);
        Task<InventorySummaryDto> GetInventory(int playerProfileId);
        Task<InventoryItemResponseDto> EquipItem(int actorPlayerProfileId, EquipItemRequestDto request);
        Task<InventoryItemResponseDto> UnequipItem(int actorPlayerProfileId, UnequipItemRequestDto request);
        Task ConsumeItem(int actorPlayerProfileId, ConsumeItemRequestDto request);
        Task<PlayerSkinResponseDto> EquipSkin(int actorPlayerProfileId, BLL.DTOs.EquipSkinRequestDto request);
        Task UnequipSkin(int actorPlayerProfileId, BLL.DTOs.UnequipSkinRequestDto request);
    }
}
