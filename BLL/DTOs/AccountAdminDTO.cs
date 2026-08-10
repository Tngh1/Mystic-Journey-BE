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
        public string? BanReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public int? PlayerProfileId { get; set; }
        public string? PlayerDisplayName { get; set; }
        public string? PlayerClass { get; set; }
        public int? PlayerLevel { get; set; }
    }

    // CreateAccountAdminRequestDto / UpdateAccountAdminRequestDto đã bỏ cùng với role
    // SuperAdmin: chúng chỉ dùng cho màn tạo/sửa tài khoản Admin. Riêng RoleId trong
    // request update là đường nâng quyền, không để lại dạng dead code.

    public class BanAccountRequestDto
    {
        [StringLength(500)]
        public string? BanReason { get; set; }
    }
}
