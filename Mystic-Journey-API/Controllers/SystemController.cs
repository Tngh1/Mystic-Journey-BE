using BLL.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    // Executes controller base operation.
    [ApiController]
    public class SystemController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        // Initializes a new instance of SystemController with dependencies: configuration.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public SystemController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public class UpdateGameVersionDto
        {
            public string? MinRequiredVersion { get; set; }
            public string? LatestVersion { get; set; }
            public string? DownloadUrl { get; set; }
            public bool? ForceUpdate { get; set; }
        }

        // ─── Guest APIs ───────────────────────────────────────────────────────
        [AllowAnonymous]
        [HttpGet("version")]
        // Returns the minimum required and latest client build versions along with update URLs.
        public IActionResult GetVersion()
        {
            var minVer = _configuration["GameVersion:MinRequiredVersion"] ?? "1.0.0"; // Minimum version supported
            var latestVer = _configuration["GameVersion:LatestVersion"] ?? "1.0.0"; // Latest live version
            var downloadUrl = _configuration["GameVersion:DownloadUrl"] ?? ""; // Client installer/patch download URL
            var forceUpdateStr = _configuration["GameVersion:ForceUpdate"] ?? "true"; // Flag enforcing client update

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Game version retrieved successfully.",
                Data = new
                {
                    MinRequiredVersion = minVer,
                    LatestVersion = latestVer,
                    DownloadUrl = downloadUrl,
                    ForceUpdate = bool.TryParse(forceUpdateStr, out var force) && force
                }
            });
        }

        // ─── Admin APIs ───────────────────────────────────────────────────────
        [Authorize(Roles = "Admin")]
        [HttpPut("version")]
        // Updates dynamic game version configuration and client download links.
        public IActionResult UpdateVersion([FromBody] UpdateGameVersionDto dto)
        {
            if (!string.IsNullOrEmpty(dto.MinRequiredVersion))
                _configuration["GameVersion:MinRequiredVersion"] = dto.MinRequiredVersion; // Apply min version

            if (!string.IsNullOrEmpty(dto.LatestVersion))
                _configuration["GameVersion:LatestVersion"] = dto.LatestVersion; // Apply latest version

            if (dto.DownloadUrl != null)
                _configuration["GameVersion:DownloadUrl"] = dto.DownloadUrl; // Apply patch link

            if (dto.ForceUpdate.HasValue)
                _configuration["GameVersion:ForceUpdate"] = dto.ForceUpdate.Value.ToString(); // Apply force update flag

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Game version configuration updated successfully.",
                Data = new
                {
                    MinRequiredVersion = _configuration["GameVersion:MinRequiredVersion"],
                    LatestVersion = _configuration["GameVersion:LatestVersion"],
                    DownloadUrl = _configuration["GameVersion:DownloadUrl"],
                    ForceUpdate = _configuration["GameVersion:ForceUpdate"]
                }
            });
        }
    }
}
