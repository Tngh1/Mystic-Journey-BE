using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    // Executes controller base operation.
    [ApiController]
    public class AdminAccountsController : ControllerBase
    {
        private readonly IAccountAdminService _accountAdminService;

        private const string ManageableRole = "Player";

        // Initializes a new instance of AdminAccountsController with dependencies: accountAdminService.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public AdminAccountsController(IAccountAdminService accountAdminService)
        {
            _accountAdminService = accountAdminService;
        }


        // ─── Admin APIs ───────────────────────────────────────────────────────
        [Authorize(Roles = "Admin")]
        [HttpGet]
        // Retrieves paginated list of player accounts with active status and email/username search filters.
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] bool? isActive = null)
        {
            var result = await _accountAdminService.GetAccountsPaged(page, pageSize, search, isActive, ManageableRole); // Query accounts in database restricted to Player role
            return Ok(new ApiResponse<PagedResultDto<AccountAdminResponseDto>> { Success = true, Data = result });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        // Retrieves single user account details and security metadata.
        public async Task<IActionResult> GetById(int id)
        {
            var account = await _accountAdminService.GetAccountById(id); // Look up account row
            if (account == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Account with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            if (!IsManageable(account))
                return AdminAccountForbidden(); // Prevent modifying admin or service accounts

            return Ok(new ApiResponse<AccountAdminResponseDto> { Success = true, Data = account });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/ban")]
        // Suspends a player account, invalidates active refresh tokens, and records the ban reason.
        public async Task<IActionResult> BanAccount(int id, [FromBody] BanAccountRequestDto? request = null)
        {
            var targetAccount = await _accountAdminService.GetAccountById(id); // Verify account exists
            if (targetAccount == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Account with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            if (!IsManageable(targetAccount))
                return AdminAccountForbidden();

            var account = await _accountAdminService.BanAccount(id, request?.BanReason); // Set IsActive = false, record ban reason, and revoke sessions
            return Ok(new ApiResponse<AccountAdminResponseDto> { Success = true, Data = account });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/unban")]
        // Restores a suspended player account to active standing.
        public async Task<IActionResult> UnbanAccount(int id)
        {
            var targetAccount = await _accountAdminService.GetAccountById(id); // Verify account exists
            if (targetAccount == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Account with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            if (!IsManageable(targetAccount))
                return AdminAccountForbidden();

            var account = await _accountAdminService.UnbanAccount(id); // Set IsActive = true and clear ban remarks
            return Ok(new ApiResponse<AccountAdminResponseDto> { Success = true, Data = account });
        }

        // Checks if the target account has the Player role and can be managed.
        private static bool IsManageable(AccountAdminResponseDto account) =>
            string.Equals(account.RoleName, ManageableRole, System.StringComparison.OrdinalIgnoreCase);

        // Returns HTTP 403 Forbidden when trying to ban non-player accounts.
        private IActionResult AdminAccountForbidden() =>
            StatusCode(403, new ApiResponse<object>
            {
                Success = false,
                Message = "Only Player accounts can be managed here.",
                ErrorCode = ErrorCodes.Forbidden
            });
    }
}
