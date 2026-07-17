using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class PlayerBuff
    {
        [Key]
        public int Id { get; set; }

        public int PlayerProfileId { get; set; }
        
        [ForeignKey("PlayerProfileId")]
        public PlayerProfile? PlayerProfile { get; set; }

        [Required, MaxLength(100)]
        public string BuffName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string IconName { get; set; } = string.Empty;

        public float DurationRemaining { get; set; }

        public bool IsDebuff { get; set; }
    }
}
