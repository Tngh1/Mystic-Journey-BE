using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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
        public int? BonusCritRate { get; set; }
        public int? BonusCritDamage { get; set; }
    }

    public class CreateItemRequestDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150, ErrorMessage = "Name must not exceed 150 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description must not exceed 1000 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Type is required.")]
        public string Type { get; set; } = "Weapon";

        public string Rarity { get; set; } = "Common";
        public string Slot { get; set; } = "None";

        [Range(0, double.MaxValue, ErrorMessage = "BaseValue cannot be negative.")]
        public decimal BaseValue { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "MaxStack must be at least 1.")]
        public int MaxStack { get; set; } = 1;

        public bool IsActive { get; set; } = true;
        public string? IconUrl { get; set; }

        public int? BaseHp { get; set; }
        public int? BaseAtk { get; set; }
        public int? BaseDef { get; set; }
        public int? BonusHp { get; set; }
        public int? BonusAtk { get; set; }
        public int? BonusDef { get; set; }
        public int? BonusCritRate { get; set; }
        public int? BonusCritDamage { get; set; }
    }

    public class UpdateItemRequestDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150, ErrorMessage = "Name must not exceed 150 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description must not exceed 1000 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Type is required.")]
        public string Type { get; set; } = "Weapon";

        public string Rarity { get; set; } = "Common";
        public string Slot { get; set; } = "None";

        [Range(0, double.MaxValue, ErrorMessage = "BaseValue cannot be negative.")]
        public decimal BaseValue { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "MaxStack must be at least 1.")]
        public int MaxStack { get; set; } = 1;

        public bool IsActive { get; set; } = true;
        public string? IconUrl { get; set; }

        public int? BaseHp { get; set; }
        public int? BaseAtk { get; set; }
        public int? BaseDef { get; set; }
        public int? BonusHp { get; set; }
        public int? BonusAtk { get; set; }
        public int? BonusDef { get; set; }
        public int? BonusCritRate { get; set; }
        public int? BonusCritDamage { get; set; }
    }

    public class ItemApiResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Data { get; set; }
    }
}
