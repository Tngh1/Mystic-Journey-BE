namespace BLL.DTOs
{
    public class FriendDto
    {
        public int FriendshipId { get; set; }
        public int FriendProfileId { get; set; }
        public string FriendName { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public int FriendLevel { get; set; }
        public string FriendAvatarUrl { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        
        // New features
        public string CurrentMap { get; set; } = "World Map";
        public bool IsInDungeon { get; set; }
        public bool CanInvite { get; set; }
        public DateTime? LastOnline { get; set; }
        public bool IsOnline { get; set; }
    }

    public class PendingFriendRequestDto
    {
        public int FriendshipId { get; set; }
        public int RequesterId { get; set; }
        public string RequesterName { get; set; } = string.Empty;
        public int RequesterLevel { get; set; }
        public string RequesterAvatarUrl { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class FriendProfileDto
    {
        public int ProfileId { get; set; }
        public string CharacterName { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public int Level { get; set; }
        public int Power { get; set; }
        public string Guild { get; set; } = "No Guild";
        public string AvatarUrl { get; set; } = string.Empty;
        public string Title { get; set; } = "Novice";
        public DateTime? LastOnline { get; set; }
        public bool IsOnline { get; set; }
    }

    public enum FriendRelationshipStatus
    {
        Self,
        None,
        RequestSent,
        RequestReceived,
        Friend,
        Blocked
    }

    public class FriendSearchDto
    {
        public int ProfileId { get; set; }
        public string CharacterName { get; set; } = string.Empty;
        public int Level { get; set; }
        public string Class { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public int Power { get; set; }
        public string GuildName { get; set; } = "No Guild";
        public bool IsOnline { get; set; }
        public FriendRelationshipStatus RelationshipStatus { get; set; }
    }
}
