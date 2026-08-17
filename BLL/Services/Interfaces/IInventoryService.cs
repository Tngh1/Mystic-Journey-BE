using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Initializes a new default instance of the IInventoryService class.
    public interface IInventoryService
    {

        Task<InventorySummaryDto> GetInventory(int playerProfileId);

        Task<PlayerMeInventoryResponseDto> GetMeInventory(int playerProfileId);

        Task<InventoryActionResultDto> EquipItem(int actorPlayerProfileId, EquipItemRequestDto request);

        Task<InventoryActionResultDto> UnequipItem(int actorPlayerProfileId, UnequipItemRequestDto request);

        Task<ConsumeItemResultDto> ConsumeItem(int actorPlayerProfileId, ConsumeItemRequestDto request);

        Task<InventoryItemResponseDto> AddItemToInventory(int playerProfileId, int itemId, int quantity);

        Task<PlayerSkinResponseDto> EquipSkin(int actorPlayerProfileId, BLL.DTOs.EquipSkinRequestDto request);

        Task UnequipSkin(int actorPlayerProfileId, BLL.DTOs.UnequipSkinRequestDto request);
    }
}
