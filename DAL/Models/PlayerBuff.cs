using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    // Initializes a new default instance of the PlayerBuff class.
    public class PlayerBuff
    {
        // Executes id operation.
        [Key]
        public int Id { get; set; }

        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }

        // Executes player profile operation.
        [ForeignKey("PlayerProfileId")]
        public PlayerProfile? PlayerProfile { get; set; }

        // Executes buff name operation.
        [Required, MaxLength(100)]
        public string BuffName { get; set; } = string.Empty;

        // Executes icon name operation.
        [MaxLength(100)]
        public string IconName { get; set; } = string.Empty;

        // Executes duration remaining operation.
        public float DurationRemaining { get; set; }

        // Executes is debuff operation.
        public bool IsDebuff { get; set; }
    }
}
