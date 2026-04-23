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
    public class SkillsController : ControllerBase
    {
        private readonly ISkillService _skillService;

        public SkillsController(ISkillService skillService)
        {
            _skillService = skillService;
        }

        private Guid GetAccountId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSkills()
        {
            var result = await _skillService.GetAllSkillsAsync();
            return Ok(result);
        }

        [HttpGet("class/{classType}")]
        public async Task<IActionResult> GetSkillsByClass(int classType)
        {
            var result = await _skillService.GetSkillsByClassAsync((DAL.Models.PlayerProfile.CharacterClass)classType);
            return Ok(result);
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableSkills()
        {
            var accountId = GetAccountId();
            var result = await _skillService.GetAvailableSkillsAsync(accountId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("{skillId}")]
        public async Task<IActionResult> GetSkillById(Guid skillId)
        {
            var result = await _skillService.GetSkillByIdAsync(skillId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("player")]
        public async Task<IActionResult> GetPlayerSkills()
        {
            var accountId = GetAccountId();
            var result = await _skillService.GetPlayerSkillsAsync(accountId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("equipped")]
        public async Task<IActionResult> GetEquippedSkills()
        {
            var accountId = GetAccountId();
            var result = await _skillService.GetEquippedSkillsAsync(accountId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost("unlock")]
        public async Task<IActionResult> UnlockSkill([FromBody] UnlockSkillRequestDto request)
        {
            var accountId = GetAccountId();
            var result = await _skillService.UnlockSkillAsync(accountId, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("upgrade")]
        public async Task<IActionResult> UpgradeSkill([FromBody] UpgradeSkillRequestDto request)
        {
            var accountId = GetAccountId();
            var result = await _skillService.UpgradeSkillAsync(accountId, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("equip")]
        public async Task<IActionResult> EquipSkill([FromBody] EquipSkillRequestDto request)
        {
            var accountId = GetAccountId();
            var result = await _skillService.EquipSkillAsync(accountId, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("unequip/{playerSkillId}")]
        public async Task<IActionResult> UnequipSkill(Guid playerSkillId)
        {
            var accountId = GetAccountId();
            var result = await _skillService.UnequipSkillAsync(accountId, playerSkillId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
