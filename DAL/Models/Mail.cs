using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Mail
    {
        public Guid Id { get; set; }

        public Guid PlayerProfileId { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public MailType Type { get; set; } = MailType.System;

        public decimal AttachedGold { get; set; } = 0;
        public decimal AttachedGems { get; set; } = 0;
        public Guid? AttachedItemId { get; set; }
        public Item? AttachedItem { get; set; }
        public int AttachedItemQuantity { get; set; } = 0;

        public bool IsRead { get; set; } = false;
        public bool IsClaimed { get; set; } = false;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiredAt { get; set; }

        public enum MailType
        {
            System = 0,
            Reward = 1,
            Event = 2,
            Compensation = 3
        }
    }
}