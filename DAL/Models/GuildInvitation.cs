namespace DAL.Models
{
    public class GuildInvitation
    {
        public int GuildInvitationId { get; set; }

        public int GuildId { get; set; }
        public Guild? Guild { get; set; }

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
