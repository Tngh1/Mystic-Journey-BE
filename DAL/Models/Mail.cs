using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Mail
    {
        public int MailId { get; set; }

        public int PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        // Types: System, Reward, Event, Compensation
        public string Type { get; set; } = "System";

        // Phần thưởng đính kèm (Gold, Gems, Item)
        public decimal AttachedGold { get; set; } = 0;
        public decimal AttachedGems { get; set; } = 0;

        public List<MailRewardItem> AttachedItems { get; set; } = new();

        public bool IsRead { get; set; } = false;
        public bool IsClaimed { get; set; } = false;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiredAt { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }
}