using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class FriendBlock
    {
        [Key]
        public int FriendBlockId { get; set; }

        [Required]
        public int BlockerId { get; set; }

        [ForeignKey("BlockerId")]
        public PlayerProfile? Blocker { get; set; }

        [Required]
        public int BlockedId { get; set; }

        [ForeignKey("BlockedId")]
        public PlayerProfile? Blocked { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
