namespace BLL.DTOs
{
    // Initializes a new default instance of the FriendDto class.
    public class FriendDto
    {
        // Executes friendship id operation.
        public int FriendshipId { get; set; }
        // Executes friend profile id operation.
        public int FriendProfileId { get; set; }
        // Executes friend name operation.
        public string FriendName { get; set; } = string.Empty;
        // Executes class operation.
        public string Class { get; set; } = string.Empty;
        // Executes friend level operation.
        public int FriendLevel { get; set; }
        // Executes friend avatar url operation.
        public string FriendAvatarUrl { get; set; } = string.Empty;
        // Supported friendship states: Pending or Accepted; Pending is unanswered and Accepted is an active friendship.
        public string Status { get; set; } = string.Empty;

        // Executes current map operation.
        public string CurrentMap { get; set; } = "World Map";
        // Executes is in dungeon operation.
        public bool IsInDungeon { get; set; }
        // Executes can invite operation.
        public bool CanInvite { get; set; }
        // Executes last online operation.
        public DateTime? LastOnline { get; set; }
        // Executes is online operation.
        public bool IsOnline { get; set; }
    }

    // Executes pending friend request dto operation.
    public class PendingFriendRequestDto
    {
        // Executes friendship id operation.
        public int FriendshipId { get; set; }
        // Executes requester id operation.
        public int RequesterId { get; set; }
        // Executes requester name operation.
        public string RequesterName { get; set; } = string.Empty;
        // Executes requester level operation.
        public int RequesterLevel { get; set; }
        // Executes requester avatar url operation.
        public string RequesterAvatarUrl { get; set; } = string.Empty;
        // Executes class operation.
        public string Class { get; set; } = string.Empty;
        // Executes created at operation.
        public DateTime CreatedAt { get; set; }
    }

    // Executes friend profile dto operation.
    public class FriendProfileDto
    {
        // Executes profile id operation.
        public int ProfileId { get; set; }
        // Executes character name operation.
        public string CharacterName { get; set; } = string.Empty;
        // Executes class operation.
        public string Class { get; set; } = string.Empty;
        // Executes level operation.
        public int Level { get; set; }
        // Executes power operation.
        public int Power { get; set; }
        // Executes guild operation.
        public string? Guild { get; set; }
        // Executes avatar url operation.
        public string? AvatarUrl { get; set; }
        // Executes title operation.
        public string Title { get; set; } = "Novice";
        // Executes last online operation.
        public DateTime LastOnline { get; set; }
        // Executes is online operation.
        public bool IsOnline { get; set; }
        // Executes has changed name operation.
        public bool HasChangedName { get; set; }
    }

    // Executes friend relationship status operation.
    public enum FriendRelationshipStatus
    {
        Self,
        None,
        RequestSent,
        RequestReceived,
        Friend,
        Blocked
    }

    // Executes friend search dto operation.
    public class FriendSearchDto
    {
        // Executes profile id operation.
        public int ProfileId { get; set; }
        // Executes character name operation.
        public string CharacterName { get; set; } = string.Empty;
        // Executes level operation.
        public int Level { get; set; }
        // Executes class operation.
        public string Class { get; set; } = string.Empty;
        // Executes avatar operation.
        public string Avatar { get; set; } = string.Empty;
        // Executes power operation.
        public int Power { get; set; }
        // Executes guild name operation.
        public string GuildName { get; set; } = "No Guild";
        // Executes is online operation.
        public bool IsOnline { get; set; }
        // Executes relationship status operation.
        public FriendRelationshipStatus RelationshipStatus { get; set; }
    }
}
