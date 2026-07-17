using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // ============ Achievement ============
    public class AchievementResponseDto
    {
        public int AchievementId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Type { get; set; } = "Combat";
        public string? IconUrl { get; set; }
        public string? BuffDescription { get; set; }
        public int RequiredValue { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? RewardItemId { get; set; }
        public string? RewardItemName { get; set; }
        public int RewardQuantity { get; set; }
        public decimal RewardGold { get; set; }
        public int RewardGem { get; set; }
        public int Point { get; set; }
    }

    public class UpdateAchievementRequestDto
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string Type { get; set; } = "Combat";
        public string? IconUrl { get; set; }
        public int RequiredValue { get; set; } = 1;
        public bool IsActive { get; set; } = true;
        public int? RewardItemId { get; set; }
        public int RewardQuantity { get; set; } = 1;
        public decimal RewardGold { get; set; }
        public int RewardGem { get; set; }
        public int Point { get; set; }
    }
}
