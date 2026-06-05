using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // ============ Chest ============
    public class ChestResponseDto
    {
        public int ChestId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Type { get; set; } = "Common";
        public int GoldMinReward { get; set; }
        public int GoldMaxReward { get; set; }
        public int ExperienceReward { get; set; }
        public bool IsActive { get; set; }
        public List<ChestItemResponseDto> ChestItems { get; set; } = new();
    }

    public class CreateChestRequestDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150, ErrorMessage = "Name must not exceed 150 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public string Type { get; set; } = "Common";
        public int GoldMinReward { get; set; }
        public int GoldMaxReward { get; set; }
        public int ExperienceReward { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateChestRequestDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public string Type { get; set; } = "Common";
        public int GoldMinReward { get; set; }
        public int GoldMaxReward { get; set; }
        public int ExperienceReward { get; set; }
        public bool IsActive { get; set; } = true;
    }

    // ============ ChestItem ============
    public class ChestItemResponseDto
    {
        public int ChestItemId { get; set; }
        public int ChestId { get; set; }
        public int ItemId { get; set; }
        public string? ItemName { get; set; }
        public string? ItemIconUrl { get; set; }
        public string? ItemRarity { get; set; }
        public int QuantityMin { get; set; }
        public int QuantityMax { get; set; }
        public decimal DropRate { get; set; }
        public bool IsGuaranteed { get; set; }
    }

    public class CreateChestItemRequestDto
    {
        [Required]
        public int ItemId { get; set; }

        public int QuantityMin { get; set; } = 1;
        public int QuantityMax { get; set; } = 1;

        [Range(0.0, 100.0, ErrorMessage = "DropRate must be between 0 and 100.")]
        public decimal DropRate { get; set; }

        public bool IsGuaranteed { get; set; }
    }

    // ============ PlayerChest ============
    public class PlayerChestResponseDto
    {
        public int PlayerChestId { get; set; }
        public int PlayerProfileId { get; set; }
        public int ChestId { get; set; }
        public string? ChestName { get; set; }
        public string? ChestType { get; set; }
        public string? ChestIconUrl { get; set; }
        public bool IsOpened { get; set; }
        public DateTime ReceivedAt { get; set; }
        public DateTime? OpenedAt { get; set; }
    }

    public class OpenChestResponseDto
    {
        public bool Success { get; set; }
        public int GoldEarned { get; set; }
        public int ExperienceEarned { get; set; }
        public List<ChestOpenedItemDto> Items { get; set; } = new();
    }

    public class ChestOpenedItemDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string? ItemIconUrl { get; set; }
        public string Rarity { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
