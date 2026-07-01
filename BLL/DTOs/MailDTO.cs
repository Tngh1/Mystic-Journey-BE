using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // ============ GetMyMails (Danh sách mail - phân trang) ============
    public class MailSummaryDto
    {
        public int MailId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = "System";
        public bool IsRead { get; set; }
        public bool HasClaimableReward { get; set; }
        public bool IsClaimed { get; set; }

        // Số ngày còn lại trước khi mail hết hạn. Null nếu không có hạn.
        public int? RemainingDays { get; set; }

        public DateTime SentAt { get; set; }
        public DateTime? ExpiredAt { get; set; }
    }

    public class MailListPagedDto
    {
        public int TotalMails { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public List<MailSummaryDto> Items { get; set; } = new();
    }

    // ============ Mail Detail ============
    public class MailDetailDto
    {
        public int MailId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Type { get; set; } = "System";
        public bool IsRead { get; set; }
        public bool IsClaimed { get; set; }
        public decimal AttachedGold { get; set; }
        public decimal AttachedGems { get; set; }
        public List<MailRewardItemDto> AttachedItems { get; set; } = new();
        public DateTime SentAt { get; set; }
        public DateTime? ExpiredAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }

    // ============ Mail Reward Item ============
    public class MailRewardItemDto
    {
        public int ItemId { get; set; }
        public string? ItemName { get; set; }
        public string? IconUrl { get; set; }
        public int Quantity { get; set; }
    }

    // ============ Send Mail (Admin) ============
    public class SendMailByListIdDto
    {
        [Required]
        public List<int> PlayerProfileIds { get; set; } = new();

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public string Type { get; set; } = "System";

        public decimal AttachedGold { get; set; } = 0;
        public decimal AttachedGems { get; set; } = 0;

        public List<SendMailRewardItemDto> AttachedItems { get; set; } = new();
        public DateTime? ExpiredAt { get; set; }
    }

    public class SendMailRewardItemDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ItemId must be greater than 0.")]
        public int ItemId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
    }

    public class SendMailToAllDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public string Type { get; set; } = "System";

        public decimal AttachedGold { get; set; } = 0;
        public decimal AttachedGems { get; set; } = 0;

        public List<SendMailRewardItemDto> AttachedItems { get; set; } = new();
        public DateTime? ExpiredAt { get; set; }
    }
}
