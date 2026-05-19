namespace DAL.Models
{
    public class Friend
    {
        public Guid Id { get; set; }

        public Guid RequesterId { get; set; }
        public PlayerProfile? Requester { get; set; }

        public Guid AddresseeId { get; set; }
        public PlayerProfile? Addressee { get; set; }

        // Statuses: Pending, Accepted, Rejected, Blocked
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RespondedAt { get; set; }
    }
}