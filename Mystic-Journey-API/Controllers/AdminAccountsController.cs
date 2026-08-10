using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    // Quản lý tài khoản người chơi.
    // Admin APIs: Xem danh sách/chi tiết, ban/unban tài khoản Player.
    //
    // Tài khoản Admin KHÔNG quản lý được qua API. Trước đây có Create/Update dành riêng
    // cho SuperAdmin; role đó đã bỏ nên hai endpoint ấy cũng bỏ theo. Không hạ quyền
    // chúng xuống Admin: như vậy Admin sẽ tự tạo/nâng quyền Admin khác được. Cấp Admin
    // mới làm trực tiếp trong DB.
    [Route("api/[controller]")]
    [ApiController]
    public class AdminAccountsController : ControllerBase
    {
        private readonly IAccountAdminService _accountAdminService;

        // Chỉ Player mới nằm trong phạm vi quản lý của Admin. Hằng này dùng cho cả filter
        // danh sách lẫn check quyền trên từng account, để hai đường không lệch nhau.
        private const string ManageableRole = "Player";

        public AdminAccountsController(IAccountAdminService accountAdminService)
        {
            _accountAdminService = accountAdminService;
        }

        // ═══════════════════════════════════════════════════════════════════════
        // ADMIN APIs
        // ═══════════════════════════════════════════════════════════════════════

        // ── GET /api/adminaccounts ─────────────────────────────────
        // Lấy danh sách accounts Player có phân trang và lọc.
        // Query: page, pageSize, search, isActive.
        // roleName không còn là tham số: danh sách luôn khoá ở Player.
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] bool? isActive = null)
        {
            var result = await _accountAdminService.GetAccountsPaged(page, pageSize, search, isActive, ManageableRole);
            return Ok(new ApiResponse<PagedResultDto<AccountAdminResponseDto>> { Success = true, Data = result });
        }

        // ── GET /api/adminaccounts/{id} ───────────────────────────
        // Lấy chi tiết account theo ID. Account Admin trả 403 chứ không trả dữ liệu.
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var account = await _accountAdminService.GetAccountById(id);
            if (account == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Account with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            if (!IsManageable(account))
                return AdminAccountForbidden();

            return Ok(new ApiResponse<AccountAdminResponseDto> { Success = true, Data = account });
        }

        // ── POST /api/adminaccounts/{id}/ban ──────────────────────
        // Ban tài khoản Player.
        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/ban")]
        // Body là optional: reason có thể bỏ trống, và caller cũ không gửi body vẫn ban được.
        public async Task<IActionResult> BanAccount(int id, [FromBody] BanAccountRequestDto? request = null)
        {
            var targetAccount = await _accountAdminService.GetAccountById(id);
            if (targetAccount == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Account with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            if (!IsManageable(targetAccount))
                return AdminAccountForbidden();

            var account = await _accountAdminService.BanAccount(id, request?.BanReason);
            return Ok(new ApiResponse<AccountAdminResponseDto> { Success = true, Data = account });
        }

        // ── POST /api/adminaccounts/{id}/unban ────────────────────
        // Unban tài khoản Player.
        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/unban")]
        public async Task<IActionResult> UnbanAccount(int id)
        {
            var targetAccount = await _accountAdminService.GetAccountById(id);
            if (targetAccount == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Account with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            if (!IsManageable(targetAccount))
                return AdminAccountForbidden();

            var account = await _accountAdminService.UnbanAccount(id);
            return Ok(new ApiResponse<AccountAdminResponseDto> { Success = true, Data = account });
        }

        // Chỉ Player là quản lý được. Viết dạng allow-list (phải BẰNG "Player") thay vì
        // deny-list ("khác Admin") để role lạ trong DB mặc định bị chặn, chứ không mặc
        // định cho qua. Bản cũ dùng deny-list và liệt kê tay "Admin"/"SuperAdmin"/"Super Admin".
        // string.Equals static để RoleName null (Role chưa Include) trả false thay vì NRE.
        private static bool IsManageable(AccountAdminResponseDto account) =>
            string.Equals(account.RoleName, ManageableRole, System.StringComparison.OrdinalIgnoreCase);

        private IActionResult AdminAccountForbidden() =>
            StatusCode(403, new ApiResponse<object>
            {
                Success = false,
                Message = "Only Player accounts can be managed here.",
                ErrorCode = ErrorCodes.Forbidden
            });
    }
}
