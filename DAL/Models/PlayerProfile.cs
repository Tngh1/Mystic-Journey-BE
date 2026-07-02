using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class PlayerProfile
    {
        public int PlayerProfileId { get; set; }

        public int AccountId { get; set; }
        public Account? Account { get; set; }

        [Required, MaxLength(100)]
        public string DisplayName { get; set; } = string.Empty;

        public string AvatarUrl { get; set; } = string.Empty;

        // Classes: Knight, Archer, Mage
        public string Class { get; set; } = string.Empty;

        public int Level { get; set; } = 1;
        public int ExperiencePoints { get; set; } = 0;

        public decimal Gold { get; set; } = 0;
        public decimal Gems { get; set; } = 0;
        public int CurrentEnergy { get; set; } = 100;
        public int MaxEnergy { get; set; } = 100;
        public DateTime LastEnergyUpdateTime { get; set; } = DateTime.UtcNow;
        public DateTime? LastFreeGachaTime { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public int TotalDungeonClears { get; set; } = 0;

        public string LastMapName { get; set; } = string.Empty;
        public double PositionX { get; set; } = 0;
        public double PositionY { get; set; } = 0;

        public PlayerStat? PlayerStats { get; set; }

        public ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
        public ICollection<PlayerSkill> PlayerSkills { get; set; } = new List<PlayerSkill>();
        public ICollection<PlayerQuest> PlayerQuests { get; set; } = new List<PlayerQuest>();
        public ICollection<Mail> Mails { get; set; } = new List<Mail>();
        public ICollection<PlayerAchievement> PlayerAchievements { get; set; } = new List<PlayerAchievement>();
    }
}