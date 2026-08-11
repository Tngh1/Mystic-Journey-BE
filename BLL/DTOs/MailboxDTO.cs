using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // ============ GetMyMailboxes (Danh sách thư - phân trang) ============
    public class MailboxSummaryDto
    {
        public int MailboxId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = "System";
        public bool IsRead { get; set; }
        public bool HasClaimableReward { get; set; }
        public bool IsClaimed { get; set; }

        // Số ngày còn lại trước khi thư hết hạn. Null nếu không có hạn.
        public int? RemainingDays { get; set; }

        public DateTime SentAt { get; set; }
        public DateTime? ExpiredAt { get; set; }
    }

    public class MailboxListPagedDto
    {
        public int TotalMailboxes { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public List<MailboxSummaryDto> Items { get; set; } = new();
    }

    // ============ Mailbox Detail ============
    public class MailboxDetailDto
    {
        public int MailboxId { get; set; }
        public int PlayerProfileId { get; set; }
        public string? PlayerName { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Type { get; set; } = "System";
        public bool IsRead { get; set; }
        public bool IsClaimed { get; set; }
        public decimal AttachedGold { get; set; }
        public decimal AttachedGems { get; set; }
        public List<MailboxRewardItemDto> AttachedItems { get; set; } = new();
        public DateTime SentAt { get; set; }
        public DateTime? ExpiredAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }

    // ============ Mailbox Reward Item ============
    public class MailboxRewardItemDto
    {
        public int ItemId { get; set; }
        public string? ItemName { get; set; }
        public string? IconUrl { get; set; }
        public int Quantity { get; set; }
    }

    // ============ Send Mailbox (Admin) ============
    public class SendMailboxByListIdDto
    {
        [Required]
        public List<int> PlayerProfileIds { get; set; } = new();

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public string Type { get; set; } = "System";

        [Range(0, 9999, ErrorMessage = "Attached gold must be between 0 and 9999.")]
        public decimal AttachedGold { get; set; } = 0;

        [Range(0, 9999, ErrorMessage = "Attached gems must be between 0 and 9999.")]
        public decimal AttachedGems { get; set; } = 0;

        public List<SendMailboxRewardItemDto> AttachedItems { get; set; } = new();
        public DateTime? ExpiredAt { get; set; }
    }

    public class SendMailboxRewardItemDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ItemId must be greater than 0.")]
        public int ItemId { get; set; }

        [Required]
        [Range(1, 99, ErrorMessage = "Quantity must be between 1 and 99.")]
        public int Quantity { get; set; }
    }

    public class SendMailboxToAllDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public string Type { get; set; } = "System";

        [Range(0, 9999, ErrorMessage = "Attached gold must be between 0 and 9999.")]
        public decimal AttachedGold { get; set; } = 0;

        [Range(0, 9999, ErrorMessage = "Attached gems must be between 0 and 9999.")]
        public decimal AttachedGems { get; set; } = 0;

        public List<SendMailboxRewardItemDto> AttachedItems { get; set; } = new();
        public DateTime? ExpiredAt { get; set; }
    }
}
