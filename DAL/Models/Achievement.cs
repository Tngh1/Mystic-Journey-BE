using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Achievement
    {
        public int AchievementId { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Types: Combat, Exploration, Social, Collection, Progression
        public string Type { get; set; } = "Combat";

        public string? IconUrl { get; set; }

        public int RequiredValue { get; set; } = 1;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? RewardItemId { get; set; }
        public Item? RewardItem { get; set; }

        public int RewardQuantity { get; set; } = 1;

        public decimal RewardGold { get; set; } = 0;
        public int RewardGem { get; set; } = 0;

        public int Point { get; set; } = 0;

        public ICollection<PlayerAchievement> PlayerAchievements { get; set; } = new List<PlayerAchievement>();
    }
}
