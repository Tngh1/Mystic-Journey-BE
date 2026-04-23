using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BLL.DTOs
{
    public class ItemResponseDto
    {
        public Guid ItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Rarity { get; set; } = string.Empty;
        public string Slot { get; set; } = string.Empty;
        public decimal BaseValue { get; set; }
        public int MaxStack { get; set; }
        public bool IsTradable { get; set; }
        public string? IconUrl { get; set; }
    }

    public class ItemDetailResponseDto : ItemResponseDto
    {
        public EquipmentStatsDto? EquipmentStats { get; set; }
    }

    public class EquipmentStatsDto
    {
        public int HealthBonus { get; set; }
        public int ManaBonus { get; set; }
        public int StrengthBonus { get; set; }
        public int DefenseBonus { get; set; }
        public int AgilityBonus { get; set; }
        public int IntelligenceBonus { get; set; }
        public int EnduranceBonus { get; set; }
        public int LuckBonus { get; set; }
        public int AttackBonus { get; set; }
        public int CriticalRateBonus { get; set; }
        public int CriticalDamageBonus { get; set; }
        public int ArmorPenetrationBonus { get; set; }
    }

    public class CreateItemRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Type { get; set; }
        public int Rarity { get; set; }
        public int Slot { get; set; }
        public decimal BaseValue { get; set; }
        public int MaxStack { get; set; } = 1;
        public bool IsTradable { get; set; } = true;
        public string? IconUrl { get; set; }
        public EquipmentStatsDto? Stats { get; set; }
    }

    public class UpdateItemRequestDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? Type { get; set; }
        public int? Rarity { get; set; }
        public int? Slot { get; set; }
        public decimal? BaseValue { get; set; }
        public int? MaxStack { get; set; }
        public bool? IsTradable { get; set; }
        public bool? IsActive { get; set; }
        public string? IconUrl { get; set; }
    }

    public class ItemListResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<ItemResponseDto>? Items { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ItemDetailResponseDto? Item { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class ItemApiResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ItemResponseDto? Item { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ItemDetailResponseDto? Detail { get; set; }
    }
}
