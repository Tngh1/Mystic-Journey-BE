using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    // Initializes a new default instance of the Achievement class.
    public class Achievement
    {
        // Executes achievement id operation.
        public int AchievementId { get; set; }

        // Executes name operation.
        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        // Executes description operation.
        public string? Description { get; set; }

        // Executes type operation.
        public string Type { get; set; } = "Combat";

        // Executes icon url operation.
        public string? IconUrl { get; set; }

        // Executes buff description operation.
        [MaxLength(200)]
        public string? BuffDescription { get; set; }

        // Executes required value operation.
        public int RequiredValue { get; set; } = 1;

        // Executes is active operation.
        public bool IsActive { get; set; } = true;

        // Executes created at operation.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Executes reward item id operation.
        public int? RewardItemId { get; set; }
        // Executes reward item operation.
        public Item? RewardItem { get; set; }

        // Executes reward quantity operation.
        public int RewardQuantity { get; set; } = 1;

        // Executes reward gold operation.
        public decimal RewardGold { get; set; } = 0;
        // Executes reward gem operation.
        public int RewardGem { get; set; } = 0;

        // Executes point operation.
        public int Point { get; set; } = 0;

        // Executes player achievements operation.
        public ICollection<PlayerAchievement> PlayerAchievements { get; set; } = new List<PlayerAchievement>();
    }
}
