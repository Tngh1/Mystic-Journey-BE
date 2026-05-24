namespace DAL.Models
{
    public class Friend
    {
        public int Id { get; set; }

        public int RequesterId { get; set; }
        public PlayerProfile? Requester { get; set; }

        public int AddresseeId { get; set; }
        public PlayerProfile? Addressee { get; set; }

        // Statuses: Pending, Accepted, Rejected, Blocked
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RespondedAt { get; set; }
    }
}