namespace DAL.Models
{
    public class PlayerStat
    {
        public Guid Id { get; set; }

        public Guid PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        public int Health { get; set; } = 100;
        public int Mana { get; set; } = 50;
        public int Strength { get; set; } = 10;
        public int Defense { get; set; } = 10;
        public int Agility { get; set; } = 10;
        public int Intelligence { get; set; } = 10;
        public int Endurance { get; set; } = 10;
        public int Luck { get; set; } = 0;

        public int CriticalRate { get; set; } = 0;
        public int CriticalDamage { get; set; } = 0;
        public int ArmorPenetration { get; set; } = 0;

        public int SkillPoints { get; set; } = 0;
        public int TotalWins { get; set; } = 0;
        public int TotalLosses { get; set; } = 0;
        public int TotalKills { get; set; } = 0;
        public int TotalDeaths { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}