namespace DAL.Models
{
    // Initializes a new default instance of the PlayerStat class.
    public class PlayerStat
    {
        // Executes player stat id operation.
        public int PlayerStatId { get; set; }

        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes player profile operation.
        public PlayerProfile? PlayerProfile { get; set; }

        // Executes current hp operation.
        public int CurrentHp { get; set; }
        // Executes max hp operation.
        public int MaxHp { get; set; }

        // Executes atk operation.
        public int Atk { get; set; }
        // Executes def operation.
        public int Def { get; set; }

        // Executes move speed operation.
        public int MoveSpeed { get; set; }
        // Executes attack speed operation.
        public int AttackSpeed { get; set; }

        // Executes crit rate operation.
        public int CritRate { get; set; }
        // Executes crit damage operation.
        public int CritDamage { get; set; }

        // Executes damage bonus operation.
        public int DamageBonus { get; set; }

        // Executes skill points operation.
        public int SkillPoints { get; set; } = 0;
        // Executes total wins operation.
        public int TotalWins { get; set; } = 0;
        // Executes total losses operation.
        public int TotalLosses { get; set; } = 0;
        // Executes total kills operation.
        public int TotalKills { get; set; } = 0;
        // Executes total deaths operation.
        public int TotalDeaths { get; set; } = 0;

        // Executes created at operation.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // Executes updated at operation.
        public DateTime? UpdatedAt { get; set; }
    }
}
