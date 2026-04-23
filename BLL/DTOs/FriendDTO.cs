using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using static DAL.Models.Friend;

namespace BLL.DTOs
{
    public class FriendResponseDto
    {
        public Guid FriendId { get; set; }
        public Guid PlayerProfileId { get; set; }
        public string PlayerDisplayName { get; set; } = string.Empty;
        public string? PlayerAvatarUrl { get; set; }
        public int PlayerLevel { get; set; }
        public string PlayerClass { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? RespondedAt { get; set; }
    }

    public class FriendRequestResponseDto
    {
        public Guid RequestId { get; set; }
        public Guid RequesterId { get; set; }
        public string RequesterDisplayName { get; set; } = string.Empty;
        public string? RequesterAvatarUrl { get; set; }
        public int RequesterLevel { get; set; }
        public string RequesterClass { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class SendFriendRequestDto
    {
        public Guid AddresseeId { get; set; }
    }

    public class RespondFriendRequestDto
    {
        public Guid FriendId { get; set; }
        public bool Accept { get; set; }
    }

    public class FriendListResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<FriendResponseDto>? Friends { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<FriendRequestResponseDto>? PendingRequests { get; set; }
        public int TotalCount { get; set; }
    }

    public class FriendApiResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
