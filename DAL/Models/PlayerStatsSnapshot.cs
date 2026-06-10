using System;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class PlayerStatsSnapshot
    {
        public int PlayerStatsSnapshotId { get; set; }

        public int PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        // snapshot fields (equipment + base if desired)
        public int MaxHp { get; set; }
        public int Atk { get; set; }
        public int Def { get; set; }

        // scaled integers (see BLL.Utils.StatScale)
        public int MoveSpeed { get; set; }
        public int AttackSpeed { get; set; }

        public int CritRate { get; set; }
        public int CritDamage { get; set; }
        public int DamageBonus { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }
}
