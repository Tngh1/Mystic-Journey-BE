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
    public class GuildsController : ControllerBase
    {
        private readonly IGuildService _guildService;

        public GuildsController(IGuildService guildService) => _guildService = guildService;

        private int GetPlayerProfileId()
        {
            var claim = User.FindFirst("PlayerProfileId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }

        // ─── View ───────────────────────────────────────────────────────

        /// <summary>Search guild list. Filters: search (name), joinPolicy (0/1/2), minLevel</summary>
        [HttpGet]
        public async Task<IActionResult> GetGuildList(
            [FromQuery] string search = "",
            [FromQuery] int? joinPolicy = null,
            [FromQuery] int? minLevel = null)
        {
            var profileId = GetPlayerProfileId();
            var guilds = await _guildService.GetGuildListAsync(profileId, search, joinPolicy, minLevel);
            return Ok(guilds);
        }

        /// <summary>Get global top 100 guilds ranked by Total Medals</summary>
        [HttpGet("rankings")]
        public async Task<IActionResult> GetGuildRankings()
        {
            var rankings = await _guildService.GetGuildRankingsAsync(100);
            return Ok(rankings);
        }

        /// <summary>Get full guild detail including member list</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGuildDetail(int id)
        {
            var guild = await _guildService.GetGuildDetailAsync(id);
            return guild == null ? NotFound("Guild not found") : Ok(guild);
        }

        /// <summary>Get guild member list separately (for quick refresh)</summary>
        [HttpGet("{id}/members")]
        public async Task<IActionResult> GetMembers(int id)
        {
            var profileId = GetPlayerProfileId();
            try
            {
                var members = await _guildService.GetMembersAsync(profileId, id);
                return Ok(members);
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        // ─── Create / Dissolve ─────────────────────────────────────────

        /// <summary>Create a new guild. Caller becomes Leader.</summary>
        [HttpPost]
        public async Task<IActionResult> CreateGuild([FromBody] CreateGuildRequestDto request)
        {
            var profileId = GetPlayerProfileId();
            try
            {
                var guild = await _guildService.CreateGuildAsync(profileId, request);
                return Ok(guild);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        /// <summary>Dissolve guild. Leader only. All members get 24h cooldown.</summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DissolveGuild(int id)
        {
            var profileId = GetPlayerProfileId();
            var success = await _guildService.DissolveGuildAsync(profileId, id);
            return success ? Ok() : Forbid();
        }

        // ─── Join / Leave ───────────────────────────────────────────────

        /// <summary>
        /// Apply or join guild. Returns cooldown info if on leave cooldown.
        /// Response: { success, canJoin, cooldownRemainingSeconds, message }
        /// </summary>
        [HttpPost("{id}/apply")]
        public async Task<IActionResult> ApplyToGuild(int id)
        {
            var profileId = GetPlayerProfileId();
            try
            {
                var result = await _guildService.ApplyToGuildAsync(profileId, id);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        /// <summary>Leave guild. Leader must transfer leadership first.</summary>
        [HttpPost("{id}/leave")]
        public async Task<IActionResult> LeaveGuild(int id)
        {
            var profileId = GetPlayerProfileId();
            try
            {
                var result = await _guildService.LeaveGuildAsync(profileId, id);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        // ─── Applications ───────────────────────────────────────────────

        /// <summary>Get pending applications. Leader/Officer only.</summary>
        [HttpGet("{id}/applications")]
        public async Task<IActionResult> GetApplications(int id)
        {
            var profileId = GetPlayerProfileId();
            try
            {
                var apps = await _guildService.GetApplicationsAsync(profileId, id);
                return Ok(apps);
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpPost("{id}/applications/{appId}/approve")]
        public async Task<IActionResult> ApproveApplication(int id, int appId)
        {
            var profileId = GetPlayerProfileId();
            var success = await _guildService.ApproveApplicationAsync(profileId, id, appId);
            return success ? Ok() : BadRequest("Failed to approve");
        }

        [HttpPost("{id}/applications/{appId}/reject")]
        public async Task<IActionResult> RejectApplication(int id, int appId)
        {
            var profileId = GetPlayerProfileId();
            var success = await _guildService.RejectApplicationAsync(profileId, id, appId);
            return success ? Ok() : BadRequest("Failed to reject");
        }

        // ─── Member Management ──────────────────────────────────────────

        /// <summary>Invite a player. Leader/Officer only. Expires in 5 minutes.</summary>
        [HttpPost("{id}/invite")]
        public async Task<IActionResult> InviteMember(int id, [FromBody] InvitePlayerRequest request)
        {
            var profileId = GetPlayerProfileId();
            try
            {
                var success = await _guildService.InviteMemberAsync(profileId, id, request.InviteeProfileId);
                return success ? Ok() : BadRequest("Failed to invite");
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        /// <summary>Kick a member. Leader can kick all; Officer can kick Members only.</summary>
        [HttpPost("{id}/members/{memberId}/kick")]
        public async Task<IActionResult> KickMember(int id, int memberId)
        {
            var profileId = GetPlayerProfileId();
            var success = await _guildService.KickMemberAsync(profileId, id, memberId);
            return success ? Ok() : BadRequest("Failed to kick");
        }

        /// <summary>Promote Member → Officer. Leader only.</summary>
        [HttpPost("{id}/members/{memberId}/promote")]
        public async Task<IActionResult> PromoteMember(int id, int memberId)
        {
            var profileId = GetPlayerProfileId();
            var success = await _guildService.PromoteMemberAsync(profileId, id, memberId);
            return success ? Ok() : BadRequest("Failed to promote");
        }

        /// <summary>Demote Officer → Member. Leader only.</summary>
        [HttpPost("{id}/members/{memberId}/demote")]
        public async Task<IActionResult> DemoteMember(int id, int memberId)
        {
            var profileId = GetPlayerProfileId();
            var success = await _guildService.DemoteMemberAsync(profileId, id, memberId);
            return success ? Ok() : BadRequest("Failed to demote");
        }

        /// <summary>Transfer Guild Leader to another member.</summary>
        [HttpPost("{id}/transfer-leader")]
        public async Task<IActionResult> TransferLeader(int id, [FromBody] TransferLeaderRequest request)
        {
            var profileId = GetPlayerProfileId();
            var success = await _guildService.TransferLeaderAsync(profileId, id, request.NewLeaderProfileId);
            return success ? Ok() : BadRequest("Failed to transfer leadership");
        }

        // ─── Settings ───────────────────────────────────────────────────

        /// <summary>Update guild settings (Level requirement, Join policy, etc.). Leader/Officer only.</summary>
        [HttpPut("{id}/settings")]
        public async Task<IActionResult> UpdateSettings(int id, [FromBody] UpdateGuildRequestDto request)
        {
            var profileId = GetPlayerProfileId();
            var success = await _guildService.UpdateSettingsAsync(profileId, id, request);
            return success ? Ok() : Forbid();
        }

        /// <summary>Update guild notice (max 200 chars). Leader/Officer only.</summary>
        [HttpPut("{id}/notice")]
        public async Task<IActionResult> UpdateNotice(int id, [FromBody] ChangeNoticeRequest request)
        {
            var profileId = GetPlayerProfileId();
            var success = await _guildService.UpdateNoticeAsync(profileId, id, request.Notice);
            return success ? Ok() : Forbid();
        }

        /// <summary>Update guild icon/banner by preset ID. Leader only.</summary>
        [HttpPut("{id}/icon")]
        public async Task<IActionResult> UpdateIcon(int id, [FromBody] ChangeIconRequest request)
        {
            var profileId = GetPlayerProfileId();
            var success = await _guildService.UpdateIconAsync(profileId, id, request.IconId, request.BannerId);
            return success ? Ok() : Forbid();
        }

        // ─────────────────────────────────────────────────────────────────────────
        // GET /api/guilds/my-guild
        // Lấy thông tin guild hiện tại của user
        // ─────────────────────────────────────────────────────────────────────────
        [HttpGet("my-guild")]
        public async Task<IActionResult> GetMyGuild()
        {
            try
            {
                int playerProfileId = GetPlayerProfileId();
                var result = await _guildService.GetMyGuildAsync(playerProfileId);
                if (result == null)
                    return Ok(new ApiResponse<GuildDetailResponseDto> { Success = true, Message = "Not in a guild", Data = null });
                return Ok(new ApiResponse<GuildDetailResponseDto> { Success = true, Message = "Success", Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = ex.Message });
            }
        }

        // ─── Donate ─────────────────────────────────────────────────────

        /// <summary>
        /// Donate gold to guild. Earns Guild EXP + Guild Medals, personal Medals + Feats.
        /// Guild levels up only when BOTH GuildExp AND TotalMedals meet requirements.
        /// </summary>
        [HttpPost("{id}/donate")]
        public async Task<IActionResult> Donate(int id, [FromBody] DonateRequest request)
        {
            var profileId = GetPlayerProfileId();
            try
            {
                var result = await _guildService.DonateAsync(profileId, id, request.Amount);
                return Ok(result);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        /// <summary>
        /// Manually level up the guild. Leader only.
        /// </summary>
        [HttpPost("{id}/level-up")]
        public async Task<IActionResult> LevelUp(int id)
        {
            var profileId = GetPlayerProfileId();
            try
            {
                var success = await _guildService.LevelUpAsync(profileId, id);
                return success ? Ok() : BadRequest("Failed to level up (insufficient requirements or not leader).");
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        // ─── Logs ───────────────────────────────────────────────────────

        /// <summary>Get last 50 guild activity log entries. Members only.</summary>
        [HttpGet("{id}/logs")]
        public async Task<IActionResult> GetLogs(int id)
        {
            var profileId = GetPlayerProfileId();
            try
            {
                var logs = await _guildService.GetLogsAsync(profileId, id);
                return Ok(logs);
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        // ─── Chat ───────────────────────────────────────────────────────

        /// <summary>Get last 50 guild chat messages. Members only.</summary>
        [HttpGet("{id}/chat")]
        public async Task<IActionResult> GetChat(int id)
        {
            var profileId = GetPlayerProfileId();
            try
            {
                var messages = await _guildService.GetGuildChatAsync(profileId, id);
                return Ok(messages);
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        /// <summary>Send a message to guild chat. Rate limited: 1 message/second.</summary>
        [HttpPost("{id}/chat")]
        public async Task<IActionResult> SendChat(int id, [FromBody] SendGuildMessageRequest request)
        {
            var profileId = GetPlayerProfileId();
            try
            {
                var message = await _guildService.SendGuildMessageAsync(profileId, id, request.Content);
                return Ok(message);
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return StatusCode(429, ex.Message); }
        }
    }
}
