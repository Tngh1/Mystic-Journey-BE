using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // ============ Dungeon ============
    public class DungeonConfigResponseDto
    {
        public int DungeonConfigId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string Type { get; set; } = "Normal";
        public int LevelRequirement { get; set; }
        public int MaxMembers { get; set; }
        public int Difficulty { get; set; }
        public int RecommendedPower { get; set; }
        public int? ChestId { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateDungeonConfigRequestDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150, ErrorMessage = "Name must not exceed 150 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string Type { get; set; } = "Normal";
        public int LevelRequirement { get; set; } = 1;
        public int MaxMembers { get; set; } = 4;
        public int Difficulty { get; set; } = 1;
        public int RecommendedPower { get; set; } = 0;
        public int? ChestId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateDungeonConfigRequestDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string Type { get; set; } = "Normal";
        public int LevelRequirement { get; set; } = 1;
        public int MaxMembers { get; set; } = 4;
        public int Difficulty { get; set; } = 1;
        public int RecommendedPower { get; set; } = 0;
        public int? ChestId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    // ============ Dungeon Session ============

    /// <summary>Returned after successfully entering a dungeon (no energy consumed).</summary>
    public class EnterDungeonResponseDto
    {
        public int DungeonSessionId { get; set; }
        public int PlayerProfileId { get; set; }
        public int DungeonConfigId { get; set; }
        public string DungeonName { get; set; } = string.Empty;
        public int EnergyCost { get; set; }
        public int PlayerCurrentEnergy { get; set; }
        public DateTime EnterTime { get; set; }
        /// <summary>Always "Active" on enter.</summary>
        public string Status { get; set; } = "Active";
    }

    // ============ Dungeon Progress ============

    /// <summary>Request body for POST /session/{id}/progress</summary>
    public class UpdateDungeonProgressRequestDto
    {
        [Range(0, int.MaxValue, ErrorMessage = "MonstersKilled cannot be negative.")]
        public int MonstersKilled { get; set; } = 0;

        public bool BossKilled { get; set; } = false;

        [Range(0, 100, ErrorMessage = "CompletionPercentage must be between 0 and 100.")]
        public int CompletionPercentage { get; set; } = 0;

        /// <summary>
        /// Optional JSON string for future extensibility
        /// (e.g. floors cleared, buffs active).
        /// </summary>
        public string? ExtraData { get; set; }
    }

    /// <summary>Current progress state returned by the progress endpoint.</summary>
    public class DungeonProgressResponseDto
    {
        public int DungeonProgressId { get; set; }
        public int DungeonSessionId { get; set; }
        public int MonstersKilled { get; set; }
        public bool BossKilled { get; set; }
        public int CompletionPercentage { get; set; }
        public string? ExtraData { get; set; }
        public DateTime? UpdatedAt { get; set; }
        /// <summary>Convenience field reflecting the parent session status.</summary>
        public string SessionStatus { get; set; } = string.Empty;
    }

    // ============ Complete Dungeon ============

    /// <summary>
    /// Returned by POST /session/{id}/complete.
    /// Contains chest preview info — rewards are NOT granted yet (BR-09).
    /// </summary>
    public class CompleteDungeonResponseDto
    {
        public int DungeonSessionId { get; set; }
        public string Status { get; set; } = "Completed";
        public DateTime CompletedTime { get; set; }
        /// <summary>Chest that will be opened when the player calls claim-reward.</summary>
        public ChestResponseDto? RewardChest { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    // ============ Claim Reward ============

    /// <summary>Full reward breakdown returned by POST /session/{id}/claim-reward.</summary>
    public class ClaimDungeonRewardResponseDto
    {
        public int DungeonSessionId { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int EnergyConsumed { get; set; }
        public int GoldEarned { get; set; }
        public int ExperienceEarned { get; set; }
        public List<DungeonRewardItemDto> Items { get; set; } = new();
    }

    /// <summary>A single item line in the claim-reward response.</summary>
    public class DungeonRewardItemDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string? ItemIconUrl { get; set; }
        public string ItemType { get; set; } = string.Empty;
        public string Rarity { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
