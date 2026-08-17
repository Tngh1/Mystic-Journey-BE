using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // Initializes a new default instance of the SkinResponseDto class.
    public class SkinResponseDto
    {
        // Executes skin id operation.
        public int SkinId { get; set; }
        // Executes name operation.
        public string Name { get; set; } = string.Empty;
        // Executes description operation.
        public string? Description { get; set; }
        // Supported skin types include Armor and FullSet; the value identifies how the cosmetic is grouped and equipped.
        public string Type { get; set; } = "Armor";
        // Supported rarity values: Common, Uncommon, Rare, Epic, Legendary, or Mythic; rarity controls quality, visuals, and sorting priority.
        public string Rarity { get; set; } = "Common";
        // Executes icon url operation.
        public string? IconUrl { get; set; }
        // Executes preview url operation.
        public string? PreviewUrl { get; set; }
        // Supported currencies: Gold or Gems; the selected currency determines which player balance is charged or credited.
        public string Currency { get; set; } = "Gems";
        // Executes price operation.
        public decimal Price { get; set; }
        // Executes is for sale operation.
        public bool IsForSale { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; }
        // Executes created at operation.
        public DateTime CreatedAt { get; set; }
    }

    // Executes create skin request dto operation.
    public class CreateSkinRequestDto
    {
        // Executes name operation.
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150, ErrorMessage = "Name must not exceed 150 characters.")]
        public string Name { get; set; } = string.Empty;

        // Executes description operation.
        [StringLength(500)]
        public string? Description { get; set; }

        // Supported skin types include Armor and FullSet; the value identifies how the cosmetic is grouped and equipped.
        public string Type { get; set; } = "Armor";
        // Supported rarity values: Common, Uncommon, Rare, Epic, Legendary, or Mythic; rarity controls quality, visuals, and sorting priority.
        public string Rarity { get; set; } = "Common";
        // Executes icon url operation.
        public string? IconUrl { get; set; }
        // Executes preview url operation.
        public string? PreviewUrl { get; set; }
        // Supported currencies: Gold or Gems; the selected currency determines which player balance is charged or credited.
        public string Currency { get; set; } = "Gems";

        // Executes price operation.
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        // Executes is for sale operation.
        public bool IsForSale { get; set; } = false;
        // Executes is active operation.
        public bool IsActive { get; set; } = true;
    }

    // Executes update skin request dto operation.
    public class UpdateSkinRequestDto
    {
        // Executes name operation.
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        // Executes description operation.
        [StringLength(500)]
        public string? Description { get; set; }

        // Supported skin types include Armor and FullSet; the value identifies how the cosmetic is grouped and equipped.
        public string Type { get; set; } = "Armor";
        // Supported rarity values: Common, Uncommon, Rare, Epic, Legendary, or Mythic; rarity controls quality, visuals, and sorting priority.
        public string Rarity { get; set; } = "Common";
        // Executes icon url operation.
        public string? IconUrl { get; set; }
        // Executes preview url operation.
        public string? PreviewUrl { get; set; }
        // Supported currencies: Gold or Gems; the selected currency determines which player balance is charged or credited.
        public string Currency { get; set; } = "Gems";

        // Executes price operation.
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        // Executes is for sale operation.
        public bool IsForSale { get; set; } = false;
        // Executes is active operation.
        public bool IsActive { get; set; } = true;
    }

    // Executes player skin response dto operation.
    public class PlayerSkinResponseDto
    {
        // Executes player skin id operation.
        public int PlayerSkinId { get; set; }
        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes skin id operation.
        public int SkinId { get; set; }
        // Executes skin name operation.
        public string SkinName { get; set; } = string.Empty;
        // Executes skin description operation.
        public string? SkinDescription { get; set; }
        // Supported skin types include Armor and FullSet; the value identifies how the cosmetic is grouped and equipped.
        public string SkinType { get; set; } = string.Empty;
        // Supported rarity values: Common, Uncommon, Rare, Epic, Legendary, or Mythic; rarity controls quality, visuals, and sorting priority.
        public string SkinRarity { get; set; } = string.Empty;
        // Executes icon url operation.
        public string? IconUrl { get; set; }
        // Executes preview url operation.
        public string? PreviewUrl { get; set; }
        // Executes is equipped operation.
        public bool IsEquipped { get; set; }
        // Executes unlocked at operation.
        public DateTime UnlockedAt { get; set; }
    }

    // Executes equip skin request dto operation.
    public class EquipSkinRequestDto
    {
        // Executes player skin id operation.
        [Required]
        public int PlayerSkinId { get; set; }

        // Executes is equipped operation.
        public bool IsEquipped { get; set; }
    }

}
