namespace DAL.Models
{
    // Initializes a new default instance of the GuildInvitation class.
    public class GuildInvitation
    {
        // Executes guild invitation id operation.
        public int GuildInvitationId { get; set; }

        // Executes guild id operation.
        public int GuildId { get; set; }
        // Executes guild operation.
        public Guild? Guild { get; set; }

        // Executes inviter id operation.
        public int InviterId { get; set; }
        // Executes inviter operation.
        public PlayerProfile? Inviter { get; set; }

        // Executes invitee id operation.
        public int InviteeId { get; set; }
        // Executes invitee operation.
        public PlayerProfile? Invitee { get; set; }

        // Supported guild request states: Pending, Accepted, Declined, or Expired; only Pending requests can transition to a final state.
        public string Status { get; set; } = "Pending";

        // Executes created at operation.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // Executes expires at operation.
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddMinutes(5);
        // Executes responded at operation.
        public DateTime? RespondedAt { get; set; }
    }
}
