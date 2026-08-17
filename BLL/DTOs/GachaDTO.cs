using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // Initializes a new default instance of the GachaBannerResponseDto class.
    public class GachaBannerResponseDto
    {
        // Executes gacha banner id operation.
        public int GachaBannerId { get; set; }
        // Executes name operation.
        public string Name { get; set; } = string.Empty;
        // Supported gacha banner types: Standard, Limited, or Event; the type controls banner categorization and presentation.
        public string Type { get; set; } = string.Empty;
        // Executes pull cost operation.
        public int PullCost { get; set; }

        // Executes cost item id operation.
        public int? CostItemId { get; set; }

        // Executes pity limit operation.
        public int PityLimit { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; }
        // Executes start at operation.
        public DateTime StartAt { get; set; }
        // Executes end at operation.
        public DateTime EndAt { get; set; }
    }

    // Initializes a new default instance of the GachaBannerResponseDto class.
    public class GachaBannerDetailResponseDto : GachaBannerResponseDto
    {
        // Executes banner items operation.
        public List<GachaBannerItemResponseDto> BannerItems { get; set; } = new();
    }

    // Executes gacha banner item response dto operation.
    public class GachaBannerItemResponseDto
    {
        // Executes gacha banner item id operation.
        public int GachaBannerItemId { get; set; }
        // Executes item id operation.
        public int ItemId { get; set; }
        // Executes item name operation.
        public string? ItemName { get; set; }
        // Executes item icon url operation.
        public string? ItemIconUrl { get; set; }
        // Supported rarity values: Common, Uncommon, Rare, Epic, Legendary, or Mythic; rarity controls quality, visuals, and sorting priority.
        public string? ItemRarity { get; set; }
        // Executes drop rate operation.
        public decimal DropRate { get; set; }
        // Executes is featured operation.
        public bool IsFeatured { get; set; }
    }

    // Executes create gacha banner request dto operation.
    public class CreateGachaBannerRequestDto
    {
        // Executes name operation.
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;
        // Supported gacha banner types: Standard, Limited, or Event; the type controls banner categorization and presentation.
        public string Type { get; set; } = "Standard";
        // Executes pull cost operation.
        public int PullCost { get; set; } = 100;

        // Executes cost item id operation.
        public int? CostItemId { get; set; }

        // Executes pity limit operation.
        public int PityLimit { get; set; } = 90;
        // Executes is active operation.
        public bool IsActive { get; set; } = true;
        // Executes start at operation.
        public DateTime StartAt { get; set; }
        // Executes end at operation.
        public DateTime EndAt { get; set; }
    }

    // Executes update gacha banner request dto operation.
    public class UpdateGachaBannerRequestDto
    {
        // Executes name operation.
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;
        // Supported gacha banner types: Standard, Limited, or Event; the type controls banner categorization and presentation.
        public string Type { get; set; } = "Standard";
        // Executes pull cost operation.
        public int PullCost { get; set; } = 100;

        // Executes cost item id operation.
        public int? CostItemId { get; set; }

        // Executes pity limit operation.
        public int PityLimit { get; set; } = 90;
        // Executes is active operation.
        public bool IsActive { get; set; } = true;
        // Executes start at operation.
        public DateTime StartAt { get; set; }
        // Executes end at operation.
        public DateTime EndAt { get; set; }
    }

    // Executes create gacha banner item request dto operation.
    public class CreateGachaBannerItemRequestDto
    {
        // Executes item id operation.
        [Required]
        public int ItemId { get; set; }

        // Executes drop rate operation.
        [Range(0.0, 100.0)]
        public decimal DropRate { get; set; }

        // Executes is featured operation.
        public bool IsFeatured { get; set; }
    }
}
