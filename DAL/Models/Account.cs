using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string HashPassword { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public Role? Role { get; set; }
        // Web client refresh token slot
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiresAt { get; set; }
        // Game client refresh token slot (separate session)
        public string? GameRefreshToken { get; set; }
        public DateTime? GameRefreshTokenExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
        public string? BanReason { get; set; }
        public PlayerProfile? PlayerProfile { get; set; }
    }
}
