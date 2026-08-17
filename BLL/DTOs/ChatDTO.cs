using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // Initializes a new default instance of the ChatMessageResponseDto class.
    public class ChatMessageResponseDto
    {
        // Executes chat message id operation.
        public int ChatMessageId { get; set; }
        // Executes sender id operation.
        public int SenderId { get; set; }
        // Executes sender name operation.
        public string? SenderName { get; set; }
        // Executes sender avatar url operation.
        public string? SenderAvatarUrl { get; set; }
        // Executes recipient id operation.
        public int RecipientId { get; set; }
        // Executes recipient name operation.
        public string? RecipientName { get; set; }
        // Executes recipient avatar url operation.
        public string? RecipientAvatarUrl { get; set; }
        // Executes content operation.
        public string Content { get; set; } = string.Empty;
        // Executes is reported operation.
        public bool IsReported { get; set; }
        // Executes is hidden operation.
        public bool IsHidden { get; set; }
        // Executes reported by id operation.
        public int? ReportedById { get; set; }
        // Executes report reason operation.
        public string? ReportReason { get; set; }
        // Executes reported at operation.
        public DateTime? ReportedAt { get; set; }
        // Executes sent at operation.
        public DateTime SentAt { get; set; }
    }

    // Executes world chat message response dto operation.
    public class WorldChatMessageResponseDto
    {
        // Executes chat message id operation.
        public int ChatMessageId { get; set; }
        // Executes sender id operation.
        public int SenderId { get; set; }
        // Executes sender name operation.
        public string? SenderName { get; set; }
        // Executes sender avatar url operation.
        public string? SenderAvatarUrl { get; set; }
        // Executes channel operation.
        public string Channel { get; set; } = "World";
        // Executes content operation.
        public string Content { get; set; } = string.Empty;
        // Executes is reported operation.
        public bool IsReported { get; set; }
        // Executes is hidden operation.
        public bool IsHidden { get; set; }
        // Executes reported by id operation.
        public int? ReportedById { get; set; }
        // Executes report reason operation.
        public string? ReportReason { get; set; }
        // Executes reported at operation.
        public DateTime? ReportedAt { get; set; }
        // Executes sent at operation.
        public DateTime SentAt { get; set; }
    }

    // Executes send world chat message request dto operation.
    public class SendWorldChatMessageRequestDto
    {
        // Executes content operation.
        [Required(ErrorMessage = "Content is required.")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Content must be between 1 and 500 characters.")]
        public string Content { get; set; } = string.Empty;
    }

    // Executes world chat message list query dto operation.
    public class WorldChatMessageListQueryDto
    {
        // Executes page operation.
        [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0.")]
        public int Page { get; set; } = 1;

        // Executes page size operation.
        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100.")]
        public int PageSize { get; set; } = 50;
    }

    // Executes send chat message request dto operation.
    public class SendChatMessageRequestDto
    {
        // Executes recipient id operation.
        [Required(ErrorMessage = "Recipient ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Recipient ID must be greater than 0.")]
        public int RecipientId { get; set; }

        // Executes content operation.
        [Required(ErrorMessage = "Content is required.")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Content must be between 1 and 500 characters.")]
        public string Content { get; set; } = string.Empty;
    }

    // Executes chat message list query dto operation.
    public class ChatMessageListQueryDto
    {
        // Executes recipient id operation.
        [Required(ErrorMessage = "Recipient ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Recipient ID must be greater than 0.")]
        public int RecipientId { get; set; }

        // Executes page operation.
        [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0.")]
        public int Page { get; set; } = 1;

        // Executes page size operation.
        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100.")]
        public int PageSize { get; set; } = 50;
    }

    // Executes report chat message request dto operation.
    public class ReportChatMessageRequestDto
    {
        // Executes chat message id operation.
        [Range(1, int.MaxValue, ErrorMessage = "Chat message ID must be greater than 0.")]
        public int ChatMessageId { get; set; }

        // Executes reason operation.
        [StringLength(500)]
        public string? Reason { get; set; }
    }

    // Executes friend response dto operation.
    public class FriendResponseDto
    {
        // Executes friend id operation.
        public int FriendId { get; set; }
        // Executes requester id operation.
        public int RequesterId { get; set; }
        // Executes requester name operation.
        public string? RequesterName { get; set; }
        // Executes requester avatar url operation.
        public string? RequesterAvatarUrl { get; set; }
        // Executes requester level operation.
        public int? RequesterLevel { get; set; }
        // Executes addressee id operation.
        public int AddresseeId { get; set; }
        // Executes addressee name operation.
        public string? AddresseeName { get; set; }
        // Executes addressee avatar url operation.
        public string? AddresseeAvatarUrl { get; set; }
        // Executes addressee level operation.
        public int? AddresseeLevel { get; set; }
        // Supported friendship states: Pending or Accepted; Pending is unanswered and Accepted is an active friendship.
        public string Status { get; set; } = "Pending";
        // Executes created at operation.
        public DateTime CreatedAt { get; set; }
        // Executes responded at operation.
        public DateTime? RespondedAt { get; set; }
    }

}
