using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Manages the player's inventory.
    // Supports viewing, equipping, unequipping, and consuming items.
    public interface IInventoryService
    {
        // ═══════════════════════════════════════════════════════════════════════
        // GAME APIs (Player)
        // ═══════════════════════════════════════════════════════════════════════

        // Returns a summary of the player's inventory (item counts, skins, bag capacity).
        Task<InventorySummaryDto> GetInventory(int playerProfileId);

        // Returns the player's full inventory with detailed item information.
        Task<PlayerMeInventoryResponseDto> GetMeInventory(int playerProfileId);

        // Equips an item from the inventory into the corresponding slot.
        // Returns the equipped item along with the player's current stats after equipping.
        Task<InventoryActionResultDto> EquipItem(int actorPlayerProfileId, EquipItemRequestDto request);

        // Unequips a currently equipped item.
        // Returns the unequipped item along with the player's current stats after unequipping.
        Task<InventoryActionResultDto> UnequipItem(int actorPlayerProfileId, UnequipItemRequestDto request);

        // Consumes a consumable item, reducing its quantity in the inventory.
        // Returns the applied effect (HP restored, Energy restored, etc.) so the client can update the UI.
        Task<ConsumeItemResultDto> ConsumeItem(int actorPlayerProfileId, ConsumeItemRequestDto request);

        // Adds an item to the player's inventory.
        Task<InventoryItemResponseDto> AddItemToInventory(int playerProfileId, int itemId, int quantity);

        // Equips a skin for the player's character.
        Task<PlayerSkinResponseDto> EquipSkin(int actorPlayerProfileId, BLL.DTOs.EquipSkinRequestDto request);

        // Unequips the currently equipped skin.
        Task UnequipSkin(int actorPlayerProfileId, BLL.DTOs.UnequipSkinRequestDto request);
    }
}
