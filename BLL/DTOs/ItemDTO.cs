using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BLL.DTOs
{
    public class ItemResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Rarity { get; set; } = string.Empty;
        public string Slot { get; set; } = string.Empty;
        public decimal BaseValue { get; set; }
        public int MaxStack { get; set; }
        public bool IsTradable { get; set; }
        public bool IsActive { get; set; }
        public string? IconUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateItemRequestDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150, ErrorMessage = "Name must not exceed 150 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description must not exceed 1000 characters.")]
        public string? Description { get; set; }

        public string Type { get; set; } = "Weapon";
        public string Rarity { get; set; } = "Common";
        public string Slot { get; set; } = "None";

        [Range(0, double.MaxValue, ErrorMessage = "BaseValue cannot be negative.")]
        public decimal BaseValue { get; set; } = 0;

        [Range(1, int.MaxValue, ErrorMessage = "MaxStack must be at least 1.")]
        public int MaxStack { get; set; } = 1;

        public bool IsTradable { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public string? IconUrl { get; set; }
    }

    public class UpdateItemRequestDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150, ErrorMessage = "Name must not exceed 150 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description must not exceed 1000 characters.")]
        public string? Description { get; set; }

        public string Type { get; set; } = "Weapon";
        public string Rarity { get; set; } = "Common";
        public string Slot { get; set; } = "None";

        [Range(0, double.MaxValue, ErrorMessage = "BaseValue cannot be negative.")]
        public decimal BaseValue { get; set; } = 0;

        [Range(1, int.MaxValue, ErrorMessage = "MaxStack must be at least 1.")]
        public int MaxStack { get; set; } = 1;

        public bool IsTradable { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public string? IconUrl { get; set; }
    }

    public class ItemApiResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Data { get; set; }
    }
}
