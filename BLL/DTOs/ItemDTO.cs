using System;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // Initializes a new default instance of the ItemResponseDto class.
    public class ItemResponseDto
    {
        // Executes item id operation.
        public int ItemId { get; set; }
        // Executes name operation.
        public string Name { get; set; } = string.Empty;
        // Executes description operation.
        public string? Description { get; set; }
        // Executes type operation.
        public string Type { get; set; } = string.Empty;
        // Supported rarity values: Common, Uncommon, Rare, Epic, Legendary, or Mythic; rarity controls quality, visuals, and sorting priority.
        public string Rarity { get; set; } = string.Empty;
        // Supported equipment slots: None, Weapon, Armor, Helmet, Gloves, Boots, Ring, Necklace, or Shield.
        public string Slot { get; set; } = string.Empty;
        // Executes base value operation.
        public decimal BaseValue { get; set; }
        // Executes corruption reduction operation.
        public float CorruptionReduction { get; set; }
        // Executes max stack operation.
        public int MaxStack { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; }
        // Executes icon url operation.
        public string? IconUrl { get; set; }
        // Executes created at operation.
        public DateTime CreatedAt { get; set; }

        // Executes base hp operation.
        public int? BaseHp { get; set; }
        // Executes base atk operation.
        public int? BaseAtk { get; set; }
        // Executes base def operation.
        public int? BaseDef { get; set; }
        // Executes bonus hp operation.
        public int? BonusHp { get; set; }
        // Executes bonus atk operation.
        public int? BonusAtk { get; set; }
        // Executes bonus def operation.
        public int? BonusDef { get; set; }
        // Executes bonus crit rate operation.
        public float? BonusCritRate { get; set; }
        // Executes bonus crit damage operation.
        public float? BonusCritDamage { get; set; }
    }

    // Executes update item request dto operation.
    public class UpdateItemRequestDto
    {
        // Executes name operation.
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150, ErrorMessage = "Name must not exceed 150 characters.")]
        public string Name { get; set; } = string.Empty;

        // Executes description operation.
        [StringLength(1000, ErrorMessage = "Description must not exceed 1000 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Type is required.")]
        [RegularExpression("^(Weapon|Armor|Consumable|Material|QuestItem|Currency)$",
            ErrorMessage = "Type must be Weapon, Armor, Consumable, Material, QuestItem, or Currency.")]
        // Executes type operation.
        public string Type { get; set; } = "Weapon";

        [RegularExpression("^(Common|Uncommon|Rare|Epic|Legendary|Mythic)$",
            ErrorMessage = "Rarity must be Common, Uncommon, Rare, Epic, Legendary, or Mythic.")]
        // Supported rarity values: Common, Uncommon, Rare, Epic, Legendary, or Mythic; rarity controls quality, visuals, and sorting priority.
        public string Rarity { get; set; } = "Common";

        [RegularExpression("^(None|Weapon|Armor|Helmet|Gloves|Boots|Pants|Ring|Necklace|Shield)$",
            ErrorMessage = "Slot must be None, Weapon, Armor, Helmet, Gloves, Boots, Pants, Ring, Necklace, or Shield.")]
        // Supported equipment slots: None, Weapon, Armor, Helmet, Gloves, Boots, Pants, Ring, Necklace, or Shield.
        public string Slot { get; set; } = "None";

        // Executes base value operation.
        [Range(0, double.MaxValue, ErrorMessage = "BaseValue cannot be negative.")]
        public decimal BaseValue { get; set; }

        // Executes max stack operation.
        [Range(1, int.MaxValue, ErrorMessage = "MaxStack must be at least 1.")]
        public int MaxStack { get; set; } = 1;

        // Executes corruption reduction operation.
        [Range(0, 100, ErrorMessage = "CorruptionReduction must be between 0 and 100.")]
        public float CorruptionReduction { get; set; } = 0;

        // Executes is active operation.
        public bool IsActive { get; set; } = true;
        // Executes icon url operation.
        public string? IconUrl { get; set; }

        // Executes base hp operation.
        [Range(0, int.MaxValue, ErrorMessage = "BaseHp cannot be negative.")]
        public int? BaseHp { get; set; }

        // Executes base atk operation.
        [Range(0, int.MaxValue, ErrorMessage = "BaseAtk cannot be negative.")]
        public int? BaseAtk { get; set; }

        // Executes base def operation.
        [Range(0, int.MaxValue, ErrorMessage = "BaseDef cannot be negative.")]
        public int? BaseDef { get; set; }

        // Executes bonus hp operation.
        [Range(0, int.MaxValue, ErrorMessage = "BonusHp cannot be negative.")]
        public int? BonusHp { get; set; }

        // Executes bonus atk operation.
        [Range(0, int.MaxValue, ErrorMessage = "BonusAtk cannot be negative.")]
        public int? BonusAtk { get; set; }

        // Executes bonus def operation.
        [Range(0, int.MaxValue, ErrorMessage = "BonusDef cannot be negative.")]
        public int? BonusDef { get; set; }

        // Executes bonus crit rate operation.
        [Range(0, float.MaxValue, ErrorMessage = "BonusCritRate cannot be negative.")]
        public float? BonusCritRate { get; set; }

        // Executes bonus crit damage operation.
        [Range(0, float.MaxValue, ErrorMessage = "BonusCritDamage cannot be negative.")]
        public float? BonusCritDamage { get; set; }
    }

}
