using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    // Executes controller base operation.
    [ApiController]
    public class DailyLoginRewardsController : ControllerBase
    {
        private readonly IDailyLoginRewardService _dailyLoginRewardService;

        // Initializes a new instance of DailyLoginRewardsController with dependencies: dailyLoginRewardService.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public DailyLoginRewardsController(IDailyLoginRewardService dailyLoginRewardService)
        {
            _dailyLoginRewardService = dailyLoginRewardService;
        }


        // ─── Admin APIs ───────────────────────────────────────────────────────
        [HttpGet]
        // Retrieves paginated list of daily login reward configurations for the specified month/year.
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 31,
            [FromQuery] int? month = null,
            [FromQuery] int? year = null)
        {
            var result = await _dailyLoginRewardService.GetDailyLoginRewardsPaged(page, pageSize, month, year); // Fetch reward schedule rows for the target month
            return Ok(new ApiResponse<PagedResultDto<DailyLoginRewardResponseDto>> { Success = true, Data = result });
        }

        [HttpGet("current-month")]
        // Retrieves the active month's daily login calendar rewards for players to view.
        public async Task<IActionResult> GetCurrentMonth(
            [FromQuery] int? month = null,
            [FromQuery] int? year = null)
        {
            var result = await _dailyLoginRewardService.GetCurrentMonthRewards(month, year); // Query 28-31 day reward items and amounts for the active calendar
            return Ok(new ApiResponse<List<DailyLoginRewardResponseDto>> { Success = true, Data = result });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("by-month")]
        // Admin endpoint: inspect full reward matrix for any given month.
        public async Task<IActionResult> GetByMonth(
            [FromQuery] int? month = null,
            [FromQuery] int? year = null)
        {
            var result = await _dailyLoginRewardService.GetRewardsByMonth(month, year); // Fetch all days in specified month
            return Ok(new ApiResponse<List<DailyLoginRewardResponseDto>> { Success = true, Data = result });
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}")]
        // Retrieves a single login reward definition by its ID.
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _dailyLoginRewardService.GetDailyLoginRewardById(id); // Look up specific day reward config
            if (result == null)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Daily login reward with ID {id} not found.",
                    ErrorCode = ErrorCodes.NotFound
                });

            return Ok(new ApiResponse<DailyLoginRewardResponseDto> { Success = true, Data = result });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        // Configures a new daily login reward for a specific day of the month.
        public async Task<IActionResult> Create([FromBody] CreateDailyLoginRewardRequestDto dto)
        {
            if (!ModelState.IsValid) // Validate month (1-12), day (1-31), reward type, and quantity
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    ErrorCode = ErrorCodes.ValidationError
                });

            try
            {
                var result = await _dailyLoginRewardService.CreateDailyLoginReward(dto); // Check for duplicate day entries and insert reward row
                return Ok(new ApiResponse<DailyLoginRewardResponseDto> { Success = true, Data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = "DUPLICATE_REWARD"
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        // Updates reward items, amounts, or premium bonus status for a daily login slot.
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDailyLoginRewardRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    ErrorCode = ErrorCodes.ValidationError
                });

            try
            {
                var result = await _dailyLoginRewardService.UpdateDailyLoginReward(id, dto); // Apply updates to item rewards and amounts
                return Ok(new ApiResponse<DailyLoginRewardResponseDto> { Success = true, Data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = ErrorCodes.NotFound
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        // Removes a configured login reward slot from the schedule.
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _dailyLoginRewardService.DeleteDailyLoginReward(id); // Delete reward configuration row
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Reward deleted successfully."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = ErrorCodes.NotFound
                });
            }
        }
    }
}
