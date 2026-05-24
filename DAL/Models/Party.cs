using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Party
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public int LeaderId { get; set; }
        public PlayerProfile? Leader { get; set; }

        public int MaxMembers { get; set; } = 4;
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DisbandedAt { get; set; }

        public ICollection<PartyMember> Members { get; set; } = new List<PartyMember>();
        public ICollection<PartyInvitation> Invitations { get; set; } = new List<PartyInvitation>();
    }
}
