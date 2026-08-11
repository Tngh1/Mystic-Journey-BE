using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // ============ Gacha ============
    public class GachaBannerResponseDto
    {
        public int GachaBannerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int PullCost { get; set; }

        // Item dùng để pull (gacha ticket). BR-053/BR-136: pull chỉ nhận ticket,
        // không nhận Coin/Gem/Energy. Null = banner chưa cấu hình ticket.
        public int? CostItemId { get; set; }

        public int PityLimit { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
    }

    public class GachaBannerDetailResponseDto : GachaBannerResponseDto
    {
        public List<GachaBannerItemResponseDto> BannerItems { get; set; } = new();
    }

    public class GachaBannerItemResponseDto
    {
        public int GachaBannerItemId { get; set; }
        public int ItemId { get; set; }
        public string? ItemName { get; set; }
        public string? ItemIconUrl { get; set; }
        public string? ItemRarity { get; set; }
        public decimal DropRate { get; set; }
        public bool IsFeatured { get; set; }
    }

    public class CreateGachaBannerRequestDto
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = "Standard";
        public int PullCost { get; set; } = 100;

        // BR-053/BR-136: banner có PullCost > 0 buộc phải cấu hình ticket item,
        // vì pull không được phép trừ Coin/Gem/Energy.
        public int? CostItemId { get; set; }

        public int PityLimit { get; set; } = 90;
        public bool IsActive { get; set; } = true;
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
    }

    public class UpdateGachaBannerRequestDto
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = "Standard";
        public int PullCost { get; set; } = 100;

        // BR-053/BR-136: banner có PullCost > 0 buộc phải cấu hình ticket item,
        // vì pull không được phép trừ Coin/Gem/Energy.
        public int? CostItemId { get; set; }

        public int PityLimit { get; set; } = 90;
        public bool IsActive { get; set; } = true;
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
    }

    public class CreateGachaBannerItemRequestDto
    {
        [Required]
        public int ItemId { get; set; }

        [Range(0.0, 100.0)]
        public decimal DropRate { get; set; }

        public bool IsFeatured { get; set; }
    }
}
