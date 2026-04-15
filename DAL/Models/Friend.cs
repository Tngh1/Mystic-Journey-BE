namespace DAL.Models
{
    public class Friend
    {
        public Guid Id { get; set; }

        public Guid RequesterId { get; set; }
        public PlayerProfile? Requester { get; set; }

        public Guid AddresseeId { get; set; }
        public PlayerProfile? Addressee { get; set; }

        public FriendStatus Status { get; set; } = FriendStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RespondedAt { get; set; }

        public enum FriendStatus
        {
            Pending = 0,
            Accepted = 1,
            Rejected = 2,
            Blocked = 3
        }
    }
}