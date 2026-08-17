using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // Initializes a new default instance of the MailboxSummaryDto class.
    public class MailboxSummaryDto
    {
        // Executes mailbox id operation.
        public int MailboxId { get; set; }
        // Executes title operation.
        public string Title { get; set; } = string.Empty;
        // Mailbox type is a free-form category with System as the current default; the backend does not enforce a closed allowlist.
        public string Type { get; set; } = "System";
        // Executes is read operation.
        public bool IsRead { get; set; }
        // Executes has claimable reward operation.
        public bool HasClaimableReward { get; set; }
        // Executes is claimed operation.
        public bool IsClaimed { get; set; }

        // Executes remaining days operation.
        public int? RemainingDays { get; set; }

        // Executes sent at operation.
        public DateTime SentAt { get; set; }
        // Executes expired at operation.
        public DateTime? ExpiredAt { get; set; }
    }

    // Executes mailbox list paged dto operation.
    public class MailboxListPagedDto
    {
        // Executes total mailboxes operation.
        public int TotalMailboxes { get; set; }
        // Executes page operation.
        public int Page { get; set; }
        // Executes page size operation.
        public int PageSize { get; set; }
        // Executes total pages operation.
        public int TotalPages { get; set; }
        // Executes items operation.
        public List<MailboxSummaryDto> Items { get; set; } = new();
    }

    // Executes mailbox detail dto operation.
    public class MailboxDetailDto
    {
        // Executes mailbox id operation.
        public int MailboxId { get; set; }
        // Executes player profile id operation.
        public int PlayerProfileId { get; set; }
        // Executes player name operation.
        public string? PlayerName { get; set; }
        // Executes title operation.
        public string Title { get; set; } = string.Empty;
        // Executes content operation.
        public string Content { get; set; } = string.Empty;
        // Mailbox type is a free-form category with System as the current default; the backend does not enforce a closed allowlist.
        public string Type { get; set; } = "System";
        // Executes is read operation.
        public bool IsRead { get; set; }
        // Executes is claimed operation.
        public bool IsClaimed { get; set; }
        // Executes attached gold operation.
        public decimal AttachedGold { get; set; }
        // Executes attached gems operation.
        public decimal AttachedGems { get; set; }
        // Executes attached items operation.
        public List<MailboxRewardItemDto> AttachedItems { get; set; } = new();
        // Executes sent at operation.
        public DateTime SentAt { get; set; }
        // Executes expired at operation.
        public DateTime? ExpiredAt { get; set; }
        // Executes is deleted operation.
        public bool IsDeleted { get; set; }
        // Executes deleted at operation.
        public DateTime? DeletedAt { get; set; }
    }

    // Executes mailbox reward item dto operation.
    public class MailboxRewardItemDto
    {
        // Executes item id operation.
        public int ItemId { get; set; }
        // Executes item name operation.
        public string? ItemName { get; set; }
        // Executes icon url operation.
        public string? IconUrl { get; set; }
        // Executes quantity operation.
        public int Quantity { get; set; }
    }

    // Executes send mailbox by list id dto operation.
    public class SendMailboxByListIdDto
    {
        // Executes player profile ids operation.
        [Required]
        public List<int> PlayerProfileIds { get; set; } = new();

        // Executes title operation.
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        // Executes content operation.
        [Required]
        public string Content { get; set; } = string.Empty;

        // Mailbox type is a free-form category with System as the current default; the backend does not enforce a closed allowlist.
        public string Type { get; set; } = "System";

        // Executes attached gold operation.
        [Range(0, 9999, ErrorMessage = "Attached gold must be between 0 and 9999.")]
        public decimal AttachedGold { get; set; } = 0;

        // Executes attached gems operation.
        [Range(0, 9999, ErrorMessage = "Attached gems must be between 0 and 9999.")]
        public decimal AttachedGems { get; set; } = 0;

        // Executes attached items operation.
        public List<SendMailboxRewardItemDto> AttachedItems { get; set; } = new();
        // Executes expired at operation.
        public DateTime? ExpiredAt { get; set; }
    }

    // Executes send mailbox reward item dto operation.
    public class SendMailboxRewardItemDto
    {
        // Executes item id operation.
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ItemId must be greater than 0.")]
        public int ItemId { get; set; }

        // Executes quantity operation.
        [Required]
        [Range(1, 99, ErrorMessage = "Quantity must be between 1 and 99.")]
        public int Quantity { get; set; }
    }

    // Executes send mailbox to all dto operation.
    public class SendMailboxToAllDto
    {
        // Executes title operation.
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        // Executes content operation.
        [Required]
        public string Content { get; set; } = string.Empty;

        // Mailbox type is a free-form category with System as the current default; the backend does not enforce a closed allowlist.
        public string Type { get; set; } = "System";

        // Executes attached gold operation.
        [Range(0, 9999, ErrorMessage = "Attached gold must be between 0 and 9999.")]
        public decimal AttachedGold { get; set; } = 0;

        // Executes attached gems operation.
        [Range(0, 9999, ErrorMessage = "Attached gems must be between 0 and 9999.")]
        public decimal AttachedGems { get; set; } = 0;

        // Executes attached items operation.
        public List<SendMailboxRewardItemDto> AttachedItems { get; set; } = new();
        // Executes expired at operation.
        public DateTime? ExpiredAt { get; set; }
    }
}
