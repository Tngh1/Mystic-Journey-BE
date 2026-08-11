using BLL.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mystic_Journey_API.Extensions;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemController : ControllerBase
    {
        private readonly IConfiguration _configuration;

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

        // ── GET /api/system/version ──────────────────────────────────
        // Lấy thông tin phiên bản game (MinRequiredVersion, LatestVersion, DownloadUrl, ForceUpdate)
        [AllowAnonymous]
        [HttpGet("version")]
        public IActionResult GetVersion()
        {
            var minVer = _configuration["GameVersion:MinRequiredVersion"] ?? "1.0.0";
            var latestVer = _configuration["GameVersion:LatestVersion"] ?? "1.0.0";
            var downloadUrl = _configuration["GameVersion:DownloadUrl"] ?? "";
            var forceUpdateStr = _configuration["GameVersion:ForceUpdate"] ?? "true";

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

        // ── PUT /api/system/version ──────────────────────────────────
        // Quản trị viên cập nhật cấu hình phiên bản game mới
        [Authorize(Roles = "Admin")]
        [HttpPut("version")]
        public IActionResult UpdateVersion([FromBody] UpdateGameVersionDto dto)
        {
            if (!string.IsNullOrEmpty(dto.MinRequiredVersion))
                _configuration["GameVersion:MinRequiredVersion"] = dto.MinRequiredVersion;

            if (!string.IsNullOrEmpty(dto.LatestVersion))
                _configuration["GameVersion:LatestVersion"] = dto.LatestVersion;

            if (dto.DownloadUrl != null)
                _configuration["GameVersion:DownloadUrl"] = dto.DownloadUrl;

            if (dto.ForceUpdate.HasValue)
                _configuration["GameVersion:ForceUpdate"] = dto.ForceUpdate.Value.ToString();

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
