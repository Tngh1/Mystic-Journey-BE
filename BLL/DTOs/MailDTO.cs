using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using static DAL.Models.Mail;

namespace BLL.DTOs
{
    public class MailResponseDto
    {
        public Guid MailId { get; set; }
        public Guid PlayerProfileId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal AttachedGold { get; set; }
        public decimal AttachedGems { get; set; }
        public Guid? AttachedItemId { get; set; }
        public string? AttachedItemName { get; set; }
        public int AttachedItemQuantity { get; set; }
        public bool IsRead { get; set; }
        public bool IsClaimed { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime? ExpiredAt { get; set; }
    }

    public class SendMailRequestDto
    {
        public Guid ReceiverId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int MailType { get; set; } = 0;
        public decimal? AttachedGold { get; set; }
        public decimal? AttachedGems { get; set; }
        public Guid? AttachedItemId { get; set; }
        public int AttachedItemQuantity { get; set; } = 0;
    }

    public class MailListResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<MailResponseDto>? Mails { get; set; }
        public int TotalCount { get; set; }
        public int UnreadCount { get; set; }
    }

    public class MailApiResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public MailResponseDto? Mail { get; set; }
    }
}
