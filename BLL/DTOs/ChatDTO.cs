using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // ============ ChatMessage ============
    public class ChatMessageResponseDto
    {
        public int ChatMessageId { get; set; }
        public int SenderId { get; set; }
        public string? SenderName { get; set; }
        public string? SenderAvatarUrl { get; set; }
        public int RecipientId { get; set; }
        public string? RecipientName { get; set; }
        public string? RecipientAvatarUrl { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsReported { get; set; }
        public bool IsHidden { get; set; }
        public DateTime SentAt { get; set; }
    }

    public class WorldChatMessageResponseDto
    {
        public int ChatMessageId { get; set; }
        public int SenderId { get; set; }
        public string? SenderName { get; set; }
        public string? SenderAvatarUrl { get; set; }
        public string Channel { get; set; } = "World";
        public string Content { get; set; } = string.Empty;
        public bool IsReported { get; set; }
        public bool IsHidden { get; set; }
        public int? ReportedById { get; set; }
        public string? ReportReason { get; set; }
        public DateTime? ReportedAt { get; set; }
        public DateTime SentAt { get; set; }
    }

    public class SendWorldChatMessageRequestDto
    {
        [Required(ErrorMessage = "Content is required.")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Content must be between 1 and 500 characters.")]
        public string Content { get; set; } = string.Empty;
    }

    public class WorldChatMessageListQueryDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0.")]
        public int Page { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100.")]
        public int PageSize { get; set; } = 50;
    }

    public class SendChatMessageRequestDto
    {
        [Required(ErrorMessage = "Recipient ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Recipient ID must be greater than 0.")]
        public int RecipientId { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Content must be between 1 and 500 characters.")]
        public string Content { get; set; } = string.Empty;
    }

    public class ChatMessageListQueryDto
    {
        [Required(ErrorMessage = "Recipient ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Recipient ID must be greater than 0.")]
        public int RecipientId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0.")]
        public int Page { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100.")]
        public int PageSize { get; set; } = 50;
    }

    public class ReportChatMessageRequestDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Chat message ID must be greater than 0.")]
        public int ChatMessageId { get; set; }

        [StringLength(500)]
        public string? Reason { get; set; }
    }

    // ============ Friend ============
    public class FriendResponseDto
    {
        public int FriendId { get; set; }
        public int RequesterId { get; set; }
        public string? RequesterName { get; set; }
        public string? RequesterAvatarUrl { get; set; }
        public int? RequesterLevel { get; set; }
        public int AddresseeId { get; set; }
        public string? AddresseeName { get; set; }
        public string? AddresseeAvatarUrl { get; set; }
        public int? AddresseeLevel { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; }
        public DateTime? RespondedAt { get; set; }
    }

    public class FriendRequestDto
    {
        [Required(ErrorMessage = "Addressee ID is required.")]
        public int AddresseeId { get; set; }
    }

    public class RespondFriendRequestDto
    {
        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; } = "Pending";
    }

    // ============ Friend List View ============
    public class FriendListResponseDto
    {
        public int FriendId { get; set; }
        public int PlayerProfileId { get; set; }
        public string PlayerDisplayName { get; set; } = string.Empty;
        public string? PlayerAvatarUrl { get; set; }
        public int PlayerLevel { get; set; }
        public string PlayerClass { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class FriendRequestListResponseDto
    {
        public int FriendId { get; set; }
        public int RequesterId { get; set; }
        public string RequesterName { get; set; } = string.Empty;
        public string? RequesterAvatarUrl { get; set; }
        public int? RequesterLevel { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
