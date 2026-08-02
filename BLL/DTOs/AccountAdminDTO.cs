using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    // ============ AccountAdminDto ============
    public class AccountAdminResponseDto
    {
        public int AccountId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public int? PlayerProfileId { get; set; }
        public string? PlayerDisplayName { get; set; }
        public string? PlayerClass { get; set; }
        public int? PlayerLevel { get; set; }
    }

    public class CreateAccountAdminRequestDto
    {
        [Required]
        [StringLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public int RoleId { get; set; }

        public string? DisplayName { get; set; }
        public string PlayerClass { get; set; } = "Knight";
    }

    public class UpdateAccountAdminRequestDto
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public int? RoleId { get; set; }
        public bool? IsActive { get; set; }
        public string? NewPassword { get; set; }
    }
}
