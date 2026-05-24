namespace DAL.Models
{
    public class PartyInvitation
    {
        public int Id { get; set; }

        public int PartyId { get; set; }
        public Party? Party { get; set; }

        public int InviterId { get; set; }
        public PlayerProfile? Inviter { get; set; }

        public int InviteeId { get; set; }
        public PlayerProfile? Invitee { get; set; }

        // Statuses: Pending, Accepted, Rejected, Expired
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RespondedAt { get; set; }
    }
}
