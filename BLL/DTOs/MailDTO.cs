using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    public class MailResponseDto
    {
        public int Id { get; set; }
        public int PlayerProfileId { get; set; }
        public string? PlayerName { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Type { get; set; } = "System";
        public decimal AttachedGold { get; set; }
        public decimal AttachedGems { get; set; }
        public int? AttachedItemId { get; set; }
        public string? AttachedItemName { get; set; }
        public int AttachedItemQuantity { get; set; }
        public bool IsRead { get; set; }
        public bool IsClaimed { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime? ExpiredAt { get; set; }
    }

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
        public decimal AttachedGold { get; set; }
        public decimal AttachedGems { get; set; }
        public int? AttachedItemId { get; set; }
        public int AttachedItemQuantity { get; set; }
        public DateTime? ExpiredAt { get; set; }
    }

    public class SendMailToAllDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public string Type { get; set; } = "System";
        public decimal AttachedGold { get; set; }
        public decimal AttachedGems { get; set; }
        public int? AttachedItemId { get; set; }
        public int AttachedItemQuantity { get; set; }
        public DateTime? ExpiredAt { get; set; }
    }
}
