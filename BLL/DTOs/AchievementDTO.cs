using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // Initializes a new default instance of the AchievementResponseDto class.
    public class AchievementResponseDto
    {
        // Executes achievement id operation.
        public int AchievementId { get; set; }
        // Executes name operation.
        public string Name { get; set; } = string.Empty;
        // Executes description operation.
        public string? Description { get; set; }
        // Executes type operation.
        public string Type { get; set; } = "Combat";
        // Executes icon url operation.
        public string? IconUrl { get; set; }
        // Executes buff description operation.
        public string? BuffDescription { get; set; }
        // Executes required value operation.
        public int RequiredValue { get; set; }
        // Executes is active operation.
        public bool IsActive { get; set; }
        // Executes created at operation.
        public DateTime CreatedAt { get; set; }
        // Executes reward item id operation.
        public int? RewardItemId { get; set; }
        // Executes reward item name operation.
        public string? RewardItemName { get; set; }
        // Executes reward quantity operation.
        public int RewardQuantity { get; set; }
        // Executes reward gold operation.
        public decimal RewardGold { get; set; }
        // Executes reward gem operation.
        public int RewardGem { get; set; }
        // Executes point operation.
        public int Point { get; set; }
    }

    // Executes update achievement request dto operation.
    public class UpdateAchievementRequestDto
    {
        // Executes name operation.
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        // Executes description operation.
        public string? Description { get; set; }
        // Executes type operation.
        public string Type { get; set; } = "Combat";
        // Executes icon url operation.
        public string? IconUrl { get; set; }
        // Executes required value operation.
        public int RequiredValue { get; set; } = 1;
        // Executes is active operation.
        public bool IsActive { get; set; } = true;
        // Executes reward item id operation.
        public int? RewardItemId { get; set; }
        // Executes reward quantity operation.
        public int RewardQuantity { get; set; } = 1;
        // Executes reward gold operation.
        public decimal RewardGold { get; set; }
        // Executes reward gem operation.
        public int RewardGem { get; set; }
        // Executes point operation.
        public int Point { get; set; }
    }
}
