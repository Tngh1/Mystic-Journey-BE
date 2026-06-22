using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminAccountsController : ControllerBase
    {
        private readonly IAccountAdminService _accountAdminService;

        public AdminAccountsController(IAccountAdminService accountAdminService)
        {
            _accountAdminService = accountAdminService;
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var account = await _accountAdminService.GetAccountById(id);
            if (account == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Account with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            return Ok(new ApiResponse<AccountAdminResponseDto> { Success = true, Data = account });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAccountAdminRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var account = await _accountAdminService.CreateAccount(request);
            return Ok(new ApiResponse<AccountAdminResponseDto> { Success = true, Data = account });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAccountAdminRequestDto request)
        {
            var account = await _accountAdminService.UpdateAccount(id, request);
            return Ok(new ApiResponse<AccountAdminResponseDto> { Success = true, Data = account });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] bool? isActive = null, [FromQuery] string? roleName = null)
        {
            var result = await _accountAdminService.GetAccountsPaged(page, pageSize, search, isActive, roleName);
            return Ok(new ApiResponse<PagedResultDto<AccountAdminResponseDto>> { Success = true, Data = result });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("{id}/ban")]
        public async Task<IActionResult> BanAccount(int id)
        {
            var account = await _accountAdminService.BanAccount(id);
            return Ok(new ApiResponse<AccountAdminResponseDto> { Success = true, Data = account });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("{id}/unban")]
        public async Task<IActionResult> UnbanAccount(int id)
        {
            var account = await _accountAdminService.UnbanAccount(id);
            return Ok(new ApiResponse<AccountAdminResponseDto> { Success = true, Data = account });
        }
    }
}
