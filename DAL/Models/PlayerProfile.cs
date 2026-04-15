using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class PlayerProfile
    {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }
        public Account? Account { get; set; }

        [Required, MaxLength(100)]
        public string DisplayName { get; set; } = string.Empty;

        public string AvatarUrl { get; set; } = string.Empty;

        public CharacterClass Class { get; set; } = CharacterClass.Knight;

        public int Level { get; set; } = 1;
        public int ExperiencePoints { get; set; } = 0;

        public decimal Gold { get; set; } = 0;
        public decimal Gems { get; set; } = 0;
        public int Energy { get; set; } = 100;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public PlayerStat? PlayerStats { get; set; }

        public ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
        public ICollection<PlayerSkill> PlayerSkills { get; set; } = new List<PlayerSkill>();
        public ICollection<PlayerQuest> PlayerQuests { get; set; } = new List<PlayerQuest>();
        public ICollection<Mail> Mails { get; set; } = new List<Mail>();

        public enum CharacterClass
        {
            Knight = 0,
            Archer = 1,
            Mage = 2
        }
    }
}