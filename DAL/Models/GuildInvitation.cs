namespace DAL.Models
{
    public class GuildInvitation
    {
        public Guid Id { get; set; }

        public Guid GuildId { get; set; }
        public Guild? Guild { get; set; }

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
