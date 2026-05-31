namespace DAL.Models
{
    public class PlayerStat
    {
        public int Id { get; set; }

        public int PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        public int CurrentHp { get; set; }
        public int MaxHp { get; set; }

        public int Atk { get; set; }
        public int Def { get; set; }

        public int MoveSpeed { get; set; }
        public int AttackSpeed { get; set; }

        public int CritRate { get; set; }
        public int CritDamage { get; set; }

        public int DamageBonus { get; set; }

        public int SkillPoints { get; set; } = 0;
        public int TotalWins { get; set; } = 0;
        public int TotalLosses { get; set; } = 0;
        public int TotalKills { get; set; } = 0;
        public int TotalDeaths { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}