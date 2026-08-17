using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    // Initializes a new default instance of the FriendBlock class.
    public class FriendBlock
    {
        // Executes friend block id operation.
        [Key]
        public int FriendBlockId { get; set; }

        // Executes blocker id operation.
        [Required]
        public int BlockerId { get; set; }

        // Executes blocker operation.
        [ForeignKey("BlockerId")]
        public PlayerProfile? Blocker { get; set; }

        // Executes blocked id operation.
        [Required]
        public int BlockedId { get; set; }

        // Executes blocked operation.
        [ForeignKey("BlockedId")]
        public PlayerProfile? Blocked { get; set; }

        // Executes created at operation.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
