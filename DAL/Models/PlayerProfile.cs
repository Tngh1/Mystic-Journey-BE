using System;
using System.Collections.Generic;
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
        
        public bool HasChangedName { get; set; } = false;

        // Classes: Knight, Archer, Mage
        public string Class { get; set; } = string.Empty;

        public int Level { get; set; } = 1;
        public int ExperiencePoints { get; set; } = 0;
        
        public int AvailableStatPoints { get; set; } = 0;
        [MaxLength(200)]
        public string CachedStatRolls { get; set; } = string.Empty;

        public void AddExperience(int exp)
        {
            if (exp <= 0) return;
            ExperiencePoints += exp;
            while (Level < 100 && ExperiencePoints >= RequiredTotalExperienceForLevel(Level + 1))
            {
                Level++;
                AvailableStatPoints++;
            }
        }

        public static int RequiredTotalExperienceForLevel(int level)
            => Math.Max(0, (level - 1) * 100);

        public decimal Gold { get; set; } = 0;
        public decimal Gems { get; set; } = 0;
        public int CurrentEnergy { get; set; } = 100;
        public int MaxEnergy { get; set; } = 100;
        public DateTime LastEnergyUpdateTime { get; set; } = DateTime.UtcNow;
        public DateTime? LastFreeGachaTime { get; set; }
        public DateTime LastActiveTime { get; set; } = DateTime.UtcNow;
        public DateTime? LastLeaveAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public int TotalDungeonClears { get; set; } = 0;

        public string LastMapName { get; set; } = string.Empty;
        public double PositionX { get; set; } = 0;
        public double PositionY { get; set; } = 0;

        public float CorruptionLevel { get; set; } = 0;

        public PlayerStat? PlayerStats { get; set; }

        // Medals/Feats are tracked at GuildMember level; expose a convenience projection
        // so we can map guild applications without forcing every DTO to query members.
        public int Medals => GuildMember?.Medals ?? 0;
        public int Feats => GuildMember?.Feats ?? 0;

        public ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
        public ICollection<PlayerSkill> PlayerSkills { get; set; } = new List<PlayerSkill>();
        public ICollection<PlayerQuest> PlayerQuests { get; set; } = new List<PlayerQuest>();
        public ICollection<PlayerBuff> PlayerBuffs { get; set; } = new List<PlayerBuff>();
        public ICollection<Mail> Mails { get; set; } = new List<Mail>();
        public ICollection<PlayerAchievement> PlayerAchievements { get; set; } = new List<PlayerAchievement>();
        public GuildMember? GuildMember { get; set; }
    }
}