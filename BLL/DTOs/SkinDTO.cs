using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // ============ Skin ============
    public class SkinResponseDto
    {
        public int SkinId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Type { get; set; } = "Armor";
        public string Rarity { get; set; } = "Common";
        public string? IconUrl { get; set; }
        public string? PreviewUrl { get; set; }
        public string Currency { get; set; } = "Gems";
        public decimal Price { get; set; }
        public bool IsForSale { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateSkinRequestDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150, ErrorMessage = "Name must not exceed 150 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public string Type { get; set; } = "Armor";
        public string Rarity { get; set; } = "Common";
        public string? IconUrl { get; set; }
        public string? PreviewUrl { get; set; }
        public string Currency { get; set; } = "Gems";

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public bool IsForSale { get; set; } = false;
        public bool IsActive { get; set; } = true;
    }

    public class UpdateSkinRequestDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public string Type { get; set; } = "Armor";
        public string Rarity { get; set; } = "Common";
        public string? IconUrl { get; set; }
        public string? PreviewUrl { get; set; }
        public string Currency { get; set; } = "Gems";

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public bool IsForSale { get; set; } = false;
        public bool IsActive { get; set; } = true;
    }

    // ============ PlayerSkin ============
    public class PlayerSkinResponseDto
    {
        public int PlayerSkinId { get; set; }
        public int PlayerProfileId { get; set; }
        public int SkinId { get; set; }
        public string SkinName { get; set; } = string.Empty;
        public string? SkinDescription { get; set; }
        public string SkinType { get; set; } = string.Empty;
        public string SkinRarity { get; set; } = string.Empty;
        public string? IconUrl { get; set; }
        public string? PreviewUrl { get; set; }
        public bool IsEquipped { get; set; }
        public DateTime UnlockedAt { get; set; }
    }

    public class EquipSkinRequestDto
    {
        [Required]
        public int PlayerSkinId { get; set; }

        public bool IsEquipped { get; set; }
    }

    public class PurchaseSkinRequestDto
    {
        [Required]
        public int SkinId { get; set; }
    }
}
