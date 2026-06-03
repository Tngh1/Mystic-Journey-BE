using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Account
    {
        public Guid AccountId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string EmailAddress { get; set; } = string.Empty;

        public string HashPassword { get; set; } = string.Empty;

        public string Gender { get; set; } = "Other";

        public string? PhoneNumber { get; set; }

        public DateOnly? Birthday { get; set; }

        public int RoleId { get; set; }
        public Role? Role { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLogin { get; set; }

        public bool IsActive { get; set; } = true;

        public string? EmailVerificationToken { get; set; }
        public DateTime? EmailVerificationTokenExpiry { get; set; }

        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }

        public PlayerProfile? PlayerProfile { get; set; }
    }
}
