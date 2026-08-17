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
    // Executes controller base operation.
    [ApiController]
    [Authorize]
    public class GuildsController : ControllerBase
    {
        private readonly IGuildService _guildService;

        // Initializes a new instance of GuildsController with dependencies: guildService.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public GuildsController(IGuildService guildService) => _guildService = guildService;

        // Executes get player profile id operation.
        private int GetPlayerProfileId()
        {
            var claim = User.FindFirst("PlayerProfileId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }


        // ─── Player APIs ───────────────────────────────────────────────────────
        [HttpGet]
        // Retrieves list of guilds matching search keywords, join policies, and level requirements.
        public async Task<IActionResult> GetGuildList(
            [FromQuery] string search = "",
            [FromQuery] int? joinPolicy = null,
            [FromQuery] int? minLevel = null)
        {
            var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
            var guilds = await _guildService.GetGuildListAsync(profileId, search, joinPolicy, minLevel); // Query public guild directory with filters
            return Ok(guilds); // Return HTTP 200 with matching guild entries
        }

        [HttpGet("rankings")]
        // Retrieves top 100 guilds sorted by level, power, and contribution points.
        public async Task<IActionResult> GetGuildRankings()
        {
            var rankings = await _guildService.GetGuildRankingsAsync(100); // Fetch top ranked guilds from database
            return Ok(rankings); // Return HTTP 200 with leaderboard
        }

        [HttpGet("{id}")]
        // Retrieves comprehensive guild details, notice, level, member count, and settings.
        public async Task<IActionResult> GetGuildDetail(int id)
        {
            var guild = await _guildService.GetGuildDetailAsync(id); // Fetch guild profile and metadata by guild ID
            return guild == null ? NotFound("Guild not found") : Ok(guild);
        }

        [HttpGet("{id}/members")]
        // Retrieves roster of all current guild members and their roles.
        public async Task<IActionResult> GetMembers(int id)
        {
            var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
            try
            {
                var members = await _guildService.GetMembersAsync(profileId, id); // Verify membership and return list of guild members
                return Ok(members);
            }
            catch (UnauthorizedAccessException) { return Forbid(); } // HTTP 403 if requester is not a member
        }


        [HttpPost]
        // Creates a new guild, deducts creation fee (gold), and sets caller as Guild Master.
        public async Task<IActionResult> CreateGuild([FromBody] CreateGuildRequestDto request)
        {
            var profileId = GetPlayerProfileId(); // Extract creator's profile ID from JWT claim
            try
            {
                var guild = await _guildService.CreateGuildAsync(profileId, request); // Deduct creation fee, create guild record, assign leader role
                return Ok(guild);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpDelete("{id}")]
        // Dissolves a guild (Leader only), kicking all members and removing guild records.
        public async Task<IActionResult> DissolveGuild(int id)
        {
            var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
            var success = await _guildService.DissolveGuildAsync(profileId, id); // Verify caller is leader and purge guild data
            return success ? Ok() : Forbid();
        }


        [HttpPost("{id}/apply")]
        // Submits an application to join a guild (auto-joins if policy is Open).
        public async Task<IActionResult> ApplyToGuild(int id)
        {
            var profileId = GetPlayerProfileId(); // Extract applicant's profile ID from JWT claim
            try
            {
                var result = await _guildService.ApplyToGuildAsync(profileId, id); // Check level requirements, guild capacity, and create pending application or join
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("{id}/leave")]
        // Leaves the current guild (Leaders must transfer leadership before leaving).
        public async Task<IActionResult> LeaveGuild(int id)
        {
            var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
            try
            {
                var result = await _guildService.LeaveGuildAsync(profileId, id); // Remove membership and apply leave cooldown
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }


        [HttpGet("{id}/applications")]
        // Retrieves pending join applications for guild officers/leaders to review.
        public async Task<IActionResult> GetApplications(int id)
        {
            var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
            try
            {
                var apps = await _guildService.GetApplicationsAsync(profileId, id); // Verify officer permissions and fetch pending applications
                return Ok(apps);
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpPost("{id}/applications/{appId}/approve")]
        // Approves a pending application, admitting the applicant into the guild.
        public async Task<IActionResult> ApproveApplication(int id, int appId)
        {
            var profileId = GetPlayerProfileId(); // Extract officer's profile ID from JWT claim
            var success = await _guildService.ApproveApplicationAsync(profileId, id, appId); // Verify capacity, add member, and mark application approved
            return success ? Ok() : BadRequest("Failed to approve");
        }

        [HttpPost("{id}/applications/{appId}/reject")]
        // Rejects a pending guild join application.
        public async Task<IActionResult> RejectApplication(int id, int appId)
        {
            var profileId = GetPlayerProfileId(); // Extract officer's profile ID from JWT claim
            var success = await _guildService.RejectApplicationAsync(profileId, id, appId); // Reject application and remove pending entry
            return success ? Ok() : BadRequest("Failed to reject");
        }


        [HttpPost("{id}/invite")]
        // Sends a guild invitation to an unguilded player.
        public async Task<IActionResult> InviteMember(int id, [FromBody] InvitePlayerRequest request)
        {
            var profileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
            try
            {
                var success = await _guildService.InviteMemberAsync(profileId, id, request.InviteeProfileId); // Verify permissions and send guild invite notification
                return success ? Ok() : BadRequest("Failed to invite");
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("{id}/members/{memberId}/kick")]
        // Kicks a member from the guild (Leader or Officer with higher rank).
        public async Task<IActionResult> KickMember(int id, int memberId)
        {
            var profileId = GetPlayerProfileId(); // Extract officer's profile ID from JWT claim
            var success = await _guildService.KickMemberAsync(profileId, id, memberId); // Verify rank hierarchy and remove member
            return success ? Ok() : BadRequest("Failed to kick");
        }

        [HttpPost("{id}/members/{memberId}/promote")]
        // Promotes a member to Officer rank.
        public async Task<IActionResult> PromoteMember(int id, int memberId)
        {
            var profileId = GetPlayerProfileId(); // Extract leader's profile ID from JWT claim
            var success = await _guildService.PromoteMemberAsync(profileId, id, memberId); // Verify officer slot cap and promote member
            return success ? Ok() : BadRequest("Failed to promote");
        }

        [HttpPost("{id}/members/{memberId}/demote")]
        // Demotes an Officer back to standard Member rank.
        public async Task<IActionResult> DemoteMember(int id, int memberId)
        {
            var profileId = GetPlayerProfileId(); // Extract leader's profile ID from JWT claim
            var success = await _guildService.DemoteMemberAsync(profileId, id, memberId); // Demote officer to regular member
            return success ? Ok() : BadRequest("Failed to demote");
        }

        [HttpPost("{id}/transfer-leader")]
        // Transfers Guild Master title to another existing member.
        public async Task<IActionResult> TransferLeader(int id, [FromBody] TransferLeaderRequest request)
        {
            var profileId = GetPlayerProfileId(); // Extract current leader's profile ID from JWT claim
            var success = await _guildService.TransferLeaderAsync(profileId, id, request.NewLeaderProfileId); // Swap leader role to new member and demote previous leader to officer
            return success ? Ok() : BadRequest("Failed to transfer leadership");
        }


        [HttpPut("{id}/settings")]
        // Updates guild join policy, min level requirement, and public visibility.
        public async Task<IActionResult> UpdateSettings(int id, [FromBody] UpdateGuildRequestDto request)
        {
            var profileId = GetPlayerProfileId(); // Extract leader's profile ID from JWT claim
            var success = await _guildService.UpdateSettingsAsync(profileId, id, request); // Persist updated guild configuration
            return success ? Ok() : Forbid();
        }

        [HttpPut("{id}/notice")]
        // Updates the guild bulletin notice/announcement text.
        public async Task<IActionResult> UpdateNotice(int id, [FromBody] ChangeNoticeRequest request)
        {
            var profileId = GetPlayerProfileId(); // Extract officer's profile ID from JWT claim
            var success = await _guildService.UpdateNoticeAsync(profileId, id, request.Notice); // Update daily announcement message
            return success ? Ok() : Forbid();
        }

        [HttpPut("{id}/icon")]
        // Updates the guild crest/emblem and banner visuals.
        public async Task<IActionResult> UpdateIcon(int id, [FromBody] ChangeIconRequest request)
        {
            var profileId = GetPlayerProfileId(); // Extract leader's profile ID from JWT claim
            var success = await _guildService.UpdateIconAsync(profileId, id, request.IconId, request.BannerId); // Set new crest and banner icon IDs
            return success ? Ok() : Forbid();
        }

        [HttpGet("my-guild")]
        // Retrieves the active player's joined guild summary and role.
        public async Task<IActionResult> GetMyGuild()
        {
            try
            {
                int playerProfileId = GetPlayerProfileId(); // Extract caller's profile ID from JWT claim
                var result = await _guildService.GetMyGuildAsync(playerProfileId); // Check player's guild membership and fetch guild info
                if (result == null)
                    return Ok(new ApiResponse<GuildDetailResponseDto> { Success = true, Message = "Not in a guild", Data = null });
                return Ok(new ApiResponse<GuildDetailResponseDto> { Success = true, Message = "Success", Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object> { Success = false, Message = ex.Message });
            }
        }


        [HttpPost("{id}/donate")]
        // Donates Gold or Gems to the guild fund, granting player contribution points and guild EXP.
        public async Task<IActionResult> Donate(int id, [FromBody] DonateRequest request)
        {
            var profileId = GetPlayerProfileId(); // Extract donor's profile ID from JWT claim
            try
            {
                var result = await _guildService.DonateAsync(profileId, id, request.CurrencyType, request.Amount); // Deduct currency, increment guild funds/EXP, and grant contribution tokens
                return Ok(result);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("{id}/level-up")]
        // Upgrades guild level when fund and EXP thresholds are achieved, unlocking extra member slots and buffs.
        public async Task<IActionResult> LevelUp(int id)
        {
            var profileId = GetPlayerProfileId(); // Extract leader's profile ID from JWT claim
            try
            {
                var success = await _guildService.LevelUpAsync(profileId, id); // Verify requirements, increment level, and increase member cap
                return success ? Ok() : BadRequest("Failed to level up (insufficient requirements or not leader).");
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }


        [HttpGet("{id}/logs")]
        // Retrieves chronological activity history (donations, joins, kicks, level ups).
        public async Task<IActionResult> GetLogs(int id)
        {
            var profileId = GetPlayerProfileId(); // Extract member's profile ID from JWT claim
            try
            {
                var logs = await _guildService.GetLogsAsync(profileId, id); // Fetch audit log entries for this guild
                return Ok(logs);
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }


        [HttpGet("{id}/chat")]
        // Retrieves recent messages from the private guild chat channel.
        public async Task<IActionResult> GetChat(int id)
        {
            var profileId = GetPlayerProfileId(); // Extract member's profile ID from JWT claim
            try
            {
                var messages = await _guildService.GetGuildChatAsync(profileId, id); // Query recent guild chat history
                return Ok(messages);
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        [HttpPost("{id}/chat")]
        // Posts a new message to the private guild channel and broadcasts via SignalR to guildmates.
        public async Task<IActionResult> SendChat(int id, [FromBody] SendGuildMessageRequest request)
        {
            var profileId = GetPlayerProfileId(); // Extract member's profile ID from JWT claim
            try
            {
                var message = await _guildService.SendGuildMessageAsync(profileId, id, request.Content); // Validate membership, persist chat row, and push to guild channel
                return Ok(message);
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return StatusCode(429, ex.Message); }
        }
    }
}
