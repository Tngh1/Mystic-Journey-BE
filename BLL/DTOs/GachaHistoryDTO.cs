using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // ============ GachaPullHistory ============
    public class GachaPullHistoryResponseDto
    {
        public int GachaPullHistoryId { get; set; }
        public int PlayerProfileId { get; set; }
        public int GachaBannerId { get; set; }
        public string? BannerName { get; set; }
        public int RewardItemId { get; set; }
        public string? RewardItemName { get; set; }
        public string? RewardItemIconUrl { get; set; }
        public string? RewardItemRarity { get; set; }
        public int PullCount { get; set; }
        public decimal CostSpent { get; set; }
        public DateTime PulledAt { get; set; }
    }

    public class GachaPullRequestDto
    {
        [Required]
        public int GachaBannerId { get; set; }

        [Range(1, 10)]
        public int PullCount { get; set; } = 1;
    }

    public class GachaPullResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int PulledItemId { get; set; }
        public string PulledItemName { get; set; } = string.Empty;
        public string? PulledItemIconUrl { get; set; }
        public string PulledItemRarity { get; set; } = string.Empty;
        public bool IsNew { get; set; }
        public int PityCounter { get; set; }
        public int CurrentPity { get; set; }
    }

    public class MultiPullResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<GachaPullResultDto> PulledItems { get; set; } = new();
        public decimal TotalCost { get; set; }
    }

    public class PlayerGachaStatsDto
    {
        public int PlayerProfileId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public int AccountId { get; set; }
        public int TotalPulls { get; set; }
        public decimal TotalCost { get; set; }
        public int LegendaryPulls { get; set; }
        public decimal ActualLegendaryRate { get; set; }
        public decimal SystemLegendaryRate { get; set; }
    }
}
