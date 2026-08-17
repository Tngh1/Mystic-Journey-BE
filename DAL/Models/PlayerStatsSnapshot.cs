using System;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    // Initializes a new default instance of the PlayerStatsSnapshot class.
    public class PlayerStatsSnapshot
    {
        // Executes player stats snapshot id operation.
        public int PlayerStatsSnapshotId { get; set; }

        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes player profile operation.
        public PlayerProfile? PlayerProfile { get; set; }

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

        // Executes created at operation.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // Executes updated at operation.
        public DateTime? UpdatedAt { get; set; }

        // Executes row version operation.
        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }
}
