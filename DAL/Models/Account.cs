using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Account
    {
        public Guid Id { get; set; }

        [Required, MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string EmailAddress { get; set; } = string.Empty;

        [Required]
        public string HashPassword { get; set; } = string.Empty;

        public GenderType Gender { get; set; } = GenderType.Male;

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        public DateOnly? Birthday { get; set; }

        public AccountRole Role { get; set; } = AccountRole.Player;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLogin { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public bool IsActive { get; set; } = true;

        public bool EmailConfirmed { get; set; } = false;
        public string? EmailVerificationToken { get; set; }
        public DateTime? EmailVerificationTokenExpiry { get; set; }

        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }

        public PlayerProfile? PlayerProfile { get; set; }

        public enum GenderType
        {
            Male = 0,
            Female = 1,
            Other = 2
        }

        public enum AccountRole
        {
            Player = 0,
            Admin = 1
        }
    }
}