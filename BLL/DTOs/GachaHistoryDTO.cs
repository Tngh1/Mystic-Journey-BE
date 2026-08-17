using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // Initializes a new default instance of the GachaPullHistoryResponseDto class.
    public class GachaPullHistoryResponseDto
    {
        // Executes gacha pull history id operation.
        public int GachaPullHistoryId { get; set; }
        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes gacha banner id operation.
        public int GachaBannerId { get; set; }
        // Executes banner name operation.
        public string? BannerName { get; set; }
        // Executes reward item id operation.
        public int RewardItemId { get; set; }
        // Executes reward item name operation.
        public string? RewardItemName { get; set; }
        // Executes reward item icon url operation.
        public string? RewardItemIconUrl { get; set; }
        // Executes reward item rarity operation.
        public string? RewardItemRarity { get; set; }
        // Executes pull count operation.
        public int PullCount { get; set; }
        // Executes cost spent operation.
        public decimal CostSpent { get; set; }
        // Executes pulled at operation.
        public DateTime PulledAt { get; set; }
    }

    // Executes gacha pull request dto operation.
    public class GachaPullRequestDto
    {
        // Executes gacha banner id operation.
        [Required]
        public int GachaBannerId { get; set; }

        // Executes pull count operation.
        [Range(1, 10)]
        public int PullCount { get; set; } = 1;
    }

    // Executes gacha pull result dto operation.
    public class GachaPullResultDto
    {
        // Executes success operation.
        public bool Success { get; set; }
        // Executes message operation.
        public string Message { get; set; } = string.Empty;
        // Executes pulled item id operation.
        public int PulledItemId { get; set; }
        // Executes pulled item name operation.
        public string PulledItemName { get; set; } = string.Empty;
        // Executes pulled item icon url operation.
        public string? PulledItemIconUrl { get; set; }
        // Executes pulled item rarity operation.
        public string PulledItemRarity { get; set; } = string.Empty;
        // Executes is new operation.
        public bool IsNew { get; set; }
        // Executes pity counter operation.
        public int PityCounter { get; set; }
        // Executes current pity operation.
        public int CurrentPity { get; set; }
    }

    // Executes multi pull result dto operation.
    public class MultiPullResultDto
    {
        // Executes success operation.
        public bool Success { get; set; }
        // Executes message operation.
        public string Message { get; set; } = string.Empty;
        // Executes pulled items operation.
        public List<GachaPullResultDto> PulledItems { get; set; } = new();
        // Executes total cost operation.
        public decimal TotalCost { get; set; }
    }

    // Executes player gacha stats dto operation.
    public class PlayerGachaStatsDto
    {
        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes player name operation.
        public string PlayerName { get; set; } = string.Empty;
        // Executes account id operation.
        public int AccountId { get; set; }
        // Executes total pulls operation.
        public int TotalPulls { get; set; }
        // Executes total cost operation.
        public decimal TotalCost { get; set; }
        // Executes legendary pulls operation.
        public int LegendaryPulls { get; set; }
        // Executes actual legendary rate operation.
        public decimal ActualLegendaryRate { get; set; }
        // Executes system legendary rate operation.
        public decimal SystemLegendaryRate { get; set; }
    }
}
