using System;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    public class ItemResponseDto
    {
        public int ItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Rarity { get; set; } = string.Empty;
        public string Slot { get; set; } = string.Empty;
        public decimal BaseValue { get; set; }
        public float CorruptionReduction { get; set; }
        public int MaxStack { get; set; }
        public bool IsActive { get; set; }
        public string? IconUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        public int? BaseHp { get; set; }
        public int? BaseAtk { get; set; }
        public int? BaseDef { get; set; }
        public int? BonusHp { get; set; }
        public int? BonusAtk { get; set; }
        public int? BonusDef { get; set; }
        public float? BonusCritRate { get; set; }
        public float? BonusCritDamage { get; set; }
    }

    public class UpdateItemRequestDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150, ErrorMessage = "Name must not exceed 150 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description must not exceed 1000 characters.")]
        public string? Description { get; set; }

        // Allowlist = hop cua gia tri dang co trong seed va dropdown cua admin portal.
        // "Currency" chi ton tai trong seed (Gold/Exp/Gem) nen phai giu, du FE khong cho chon.
        [Required(ErrorMessage = "Type is required.")]
        [RegularExpression("^(Weapon|Armor|Consumable|Material|QuestItem|Currency)$",
            ErrorMessage = "Type must be Weapon, Armor, Consumable, Material, QuestItem, or Currency.")]
        public string Type { get; set; } = "Weapon";

        [RegularExpression("^(Common|Uncommon|Rare|Epic|Legendary|Mythic)$",
            ErrorMessage = "Rarity must be Common, Uncommon, Rare, Epic, Legendary, or Mythic.")]
        public string Rarity { get; set; } = "Common";

        [RegularExpression("^(None|Weapon|Armor|Helmet|Gloves|Boots|Ring|Necklace)$",
            ErrorMessage = "Slot must be None, Weapon, Armor, Helmet, Gloves, Boots, Ring, or Necklace.")]
        public string Slot { get; set; } = "None";

        [Range(0, double.MaxValue, ErrorMessage = "BaseValue cannot be negative.")]
        public decimal BaseValue { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "MaxStack must be at least 1.")]
        public int MaxStack { get; set; } = 1;

        // Ty le, khong phai phan tram: InventoryService dung CorruptionLevel * CorruptionReduction.
        [Range(0, 1, ErrorMessage = "CorruptionReduction must be between 0 and 1.")]
        public float CorruptionReduction { get; set; } = 0;

        public bool IsActive { get; set; } = true;
        public string? IconUrl { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "BaseHp cannot be negative.")]
        public int? BaseHp { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "BaseAtk cannot be negative.")]
        public int? BaseAtk { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "BaseDef cannot be negative.")]
        public int? BaseDef { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "BonusHp cannot be negative.")]
        public int? BonusHp { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "BonusAtk cannot be negative.")]
        public int? BonusAtk { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "BonusDef cannot be negative.")]
        public int? BonusDef { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "BonusCritRate cannot be negative.")]
        public float? BonusCritRate { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "BonusCritDamage cannot be negative.")]
        public float? BonusCritDamage { get; set; }
    }

}
