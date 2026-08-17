namespace DAL.Models
{
    // Initializes a new default instance of the Friend class.
    public class Friend
    {
        // Executes friend id operation.
        public int FriendId { get; set; }

        // Executes requester id operation.
        public int RequesterId { get; set; }
        // Executes requester operation.
        public PlayerProfile? Requester { get; set; }

        // Executes addressee id operation.
        public int AddresseeId { get; set; }
        // Executes addressee operation.
        public PlayerProfile? Addressee { get; set; }

        // Supported friendship states: Pending or Accepted; Pending is unanswered and Accepted is an active friendship.
        public string Status { get; set; } = "Pending";

        // Executes created at operation.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // Executes responded at operation.
        public DateTime? RespondedAt { get; set; }
    }
}
