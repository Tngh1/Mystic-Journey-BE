namespace DAL.Models
{
    public class PartyInvitation
    {
        public Guid Id { get; set; }

        public Guid PartyId { get; set; }
        public Party? Party { get; set; }

        public Guid InviterId { get; set; }
        public PlayerProfile? Inviter { get; set; }

        public Guid InviteeId { get; set; }
        public PlayerProfile? Invitee { get; set; }

        // Statuses: Pending, Accepted, Rejected, Expired
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RespondedAt { get; set; }
    }
}
