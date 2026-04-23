using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BLL.DTOs
{
    public class GachaBannerResponseDto
    {
        public Guid BannerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int PullCost { get; set; }
        public int PityLimit { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
    }

    public class GachaPullResultDto
    {
        public Guid RewardItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string ItemRarity { get; set; } = string.Empty;
        public string? IconUrl { get; set; }
        public bool IsFeatured { get; set; }
        public int PullNumber { get; set; }
    }

    public class GachaPullRequestDto
    {
        public Guid BannerId { get; set; }
        public int PullCount { get; set; } = 1;
    }

    public class GachaPullHistoryResponseDto
    {
        public Guid PullId { get; set; }
        public Guid PlayerProfileId { get; set; }
        public Guid BannerId { get; set; }
        public string BannerName { get; set; } = string.Empty;
        public Guid RewardItemId { get; set; }
        public string RewardItemName { get; set; } = string.Empty;
        public string RewardItemRarity { get; set; } = string.Empty;
        public int PullCount { get; set; }
        public decimal CostSpent { get; set; }
        public DateTime PulledAt { get; set; }
    }

    public class GachaBannerListResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<GachaBannerResponseDto>? Banners { get; set; }
        public int TotalCount { get; set; }
    }

    public class GachaApiResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public GachaBannerResponseDto? Banner { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<GachaPullResultDto>? PullResults { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PlayerCurrencyResponseDto? Currency { get; set; }
    }

    public class GachaHistoryListResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<GachaPullHistoryResponseDto>? History { get; set; }
        public int TotalCount { get; set; }
    }
}
