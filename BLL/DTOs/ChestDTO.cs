using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // Initializes a new default instance of the ChestResponseDto class.
    public class ChestResponseDto
    {
        // Executes chest id operation.
        public int ChestId { get; set; }
        // Executes name operation.
        public string Name { get; set; } = string.Empty;
        // Executes description operation.
        public string? Description { get; set; }
        // Chest type is a free-form category with Common as the current default; the backend does not enforce a closed allowlist.
        public string Type { get; set; } = "Common";
        // Executes gold min reward operation.
        public int GoldMinReward { get; set; }
        // Executes gold max reward operation.
        public int GoldMaxReward { get; set; }
        // Executes experience reward operation.
        public int ExperienceReward { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; }
        // Executes chest items operation.
        public List<ChestItemResponseDto> ChestItems { get; set; } = new();
    }

    // Executes create chest request dto operation.
    public class CreateChestRequestDto
    {
        // Executes name operation.
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150, ErrorMessage = "Name must not exceed 150 characters.")]
        public string Name { get; set; } = string.Empty;

        // Executes description operation.
        [StringLength(500)]
        public string? Description { get; set; }

        // Chest type is a free-form category with Common as the current default; the backend does not enforce a closed allowlist.
        public string Type { get; set; } = "Common";
        // Executes gold min reward operation.
        public int GoldMinReward { get; set; }
        // Executes gold max reward operation.
        public int GoldMaxReward { get; set; }
        // Executes experience reward operation.
        public int ExperienceReward { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; } = true;
    }

    // Executes update chest request dto operation.
    public class UpdateChestRequestDto
    {
        // Executes name operation.
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        // Executes description operation.
        [StringLength(500)]
        public string? Description { get; set; }

        // Chest type is a free-form category with Common as the current default; the backend does not enforce a closed allowlist.
        public string Type { get; set; } = "Common";
        // Executes gold min reward operation.
        public int GoldMinReward { get; set; }
        // Executes gold max reward operation.
        public int GoldMaxReward { get; set; }
        // Executes experience reward operation.
        public int ExperienceReward { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; } = true;
    }

    // Executes chest item response dto operation.
    public class ChestItemResponseDto
    {
        // Executes chest item id operation.
        public int ChestItemId { get; set; }
        // Executes chest id operation.
        public int ChestId { get; set; }
        // Executes item id operation.
        public int ItemId { get; set; }
        // Executes item name operation.
        public string? ItemName { get; set; }
        // Executes item icon url operation.
        public string? ItemIconUrl { get; set; }
        // Supported rarity values: Common, Uncommon, Rare, Epic, Legendary, or Mythic; rarity controls quality, visuals, and sorting priority.
        public string? ItemRarity { get; set; }
        // Executes quantity min operation.
        public int QuantityMin { get; set; }
        // Executes quantity max operation.
        public int QuantityMax { get; set; }
        // Executes drop rate operation.
        public decimal DropRate { get; set; }
        // Executes is guaranteed operation.
        public bool IsGuaranteed { get; set; }
    }

    // Executes create chest item request dto operation.
    public class CreateChestItemRequestDto
    {
        // Executes item id operation.
        [Required]
        public int ItemId { get; set; }

        // Executes quantity min operation.
        public int QuantityMin { get; set; } = 1;
        // Executes quantity max operation.
        public int QuantityMax { get; set; } = 1;

        // Executes drop rate operation.
        [Range(0.0, 100.0, ErrorMessage = "DropRate must be between 0 and 100.")]
        public decimal DropRate { get; set; }

        // Executes is guaranteed operation.
        public bool IsGuaranteed { get; set; }
    }

    // Executes player chest response dto operation.
    public class PlayerChestResponseDto
    {
        // Executes player chest id operation.
        public int PlayerChestId { get; set; }
        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes chest id operation.
        public int ChestId { get; set; }
        // Executes chest name operation.
        public string? ChestName { get; set; }
        // Executes chest type operation.
        public string? ChestType { get; set; }
        // Executes chest icon url operation.
        public string? ChestIconUrl { get; set; }
        // Executes is opened operation.
        public bool IsOpened { get; set; }
        // Executes received at operation.
        public DateTime ReceivedAt { get; set; }
        // Executes opened at operation.
        public DateTime? OpenedAt { get; set; }
    }

    // Executes open chest response dto operation.
    public class OpenChestResponseDto
    {
        // Executes success operation.
        public bool Success { get; set; }
        // Executes gold earned operation.
        public int GoldEarned { get; set; }
        // Executes experience earned operation.
        public int ExperienceEarned { get; set; }
        // Executes items operation.
        public List<ChestOpenedItemDto> Items { get; set; } = new();
    }

    // Executes chest opened item dto operation.
    public class ChestOpenedItemDto
    {
        // Executes item id operation.
        public int ItemId { get; set; }
        // Executes item name operation.
        public string ItemName { get; set; } = string.Empty;
        // Executes item icon url operation.
        public string? ItemIconUrl { get; set; }
        // Supported rarity values: Common, Uncommon, Rare, Epic, Legendary, or Mythic; rarity controls quality, visuals, and sorting priority.
        public string Rarity { get; set; } = string.Empty;
        // Executes quantity operation.
        public int Quantity { get; set; }
    }
}
