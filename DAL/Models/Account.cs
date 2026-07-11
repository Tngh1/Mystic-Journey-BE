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
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime? LastSeen { get; set; }
        public bool IsActive { get; set; } = true;
        public PlayerProfile? PlayerProfile { get; set; }
    }
}
