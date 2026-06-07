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
        public string Content { get; set; } = string.Empty;
        public bool IsReported { get; set; }
        public bool IsHidden { get; set; }
        public DateTime SentAt { get; set; }
    }

    public class SendChatMessageRequestDto
    {
        [Required(ErrorMessage = "Recipient ID is required.")]
        public int RecipientId { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        [StringLength(500, ErrorMessage = "Content must not exceed 500 characters.")]
        public string Content { get; set; } = string.Empty;
    }

    public class ReportChatMessageRequestDto
    {
        [Required]
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
