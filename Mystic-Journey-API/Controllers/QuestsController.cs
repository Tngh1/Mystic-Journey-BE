using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuestsController : ControllerBase
    {
        private readonly IQuestService _questService;

        public QuestsController(IQuestService questService)
        {
            _questService = questService;
        }

        private Guid GetAccountId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllQuests()
        {
            var result = await _questService.GetAllQuestsAsync();
            return Ok(result);
        }

        [HttpGet("type/{type}")]
        public async Task<IActionResult> GetQuestsByType(int type)
        {
            var result = await _questService.GetQuestsByTypeAsync((DAL.Models.Quest.QuestType)type);
            return Ok(result);
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableQuests()
        {
            var accountId = GetAccountId();
            var result = await _questService.GetAvailableQuestsAsync(accountId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("{questId}")]
        public async Task<IActionResult> GetQuestById(Guid questId)
        {
            var result = await _questService.GetQuestByIdAsync(questId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("player")]
        public async Task<IActionResult> GetPlayerQuests()
        {
            var accountId = GetAccountId();
            var result = await _questService.GetPlayerQuestsAsync(accountId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveQuests()
        {
            var accountId = GetAccountId();
            var result = await _questService.GetActiveQuestsAsync(accountId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("completed")]
        public async Task<IActionResult> GetCompletedQuests()
        {
            var accountId = GetAccountId();
            var result = await _questService.GetCompletedQuestsAsync(accountId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost("accept")]
        public async Task<IActionResult> AcceptQuest([FromBody] AcceptQuestRequestDto request)
        {
            var accountId = GetAccountId();
            var result = await _questService.AcceptQuestAsync(accountId, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("progress")]
        public async Task<IActionResult> UpdateProgress([FromBody] UpdateQuestProgressRequestDto request)
        {
            var accountId = GetAccountId();
            var result = await _questService.UpdateProgressAsync(accountId, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("claim")]
        public async Task<IActionResult> ClaimReward([FromBody] ClaimQuestRequestDto request)
        {
            var accountId = GetAccountId();
            var result = await _questService.ClaimQuestRewardAsync(accountId, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
