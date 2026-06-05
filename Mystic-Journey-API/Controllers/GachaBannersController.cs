using BLL.DTOs;
using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using System.Threading.Tasks;

namespace Mystic_Journey_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GachaBannersController : ControllerBase
    {
        private readonly IGachaBannerService _gachaBannerService;

        public GachaBannersController(IGachaBannerService gachaBannerService)
        {
            _gachaBannerService = gachaBannerService;
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var banner = await _gachaBannerService.GetBannerById(id);
                if (banner == null)
                    return NotFound(new { message = $"Gacha banner with id {id} not found." });

                return Ok(banner);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGachaBannerRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var banner = await _gachaBannerService.CreateBanner(request);
                return Ok(banner);
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateGachaBannerRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var banner = await _gachaBannerService.UpdateBanner(id, request);
                return Ok(banner);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/items")]
        public async Task<IActionResult> AddBannerItem(int id, [FromBody] CreateGachaBannerItemRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var bannerItem = await _gachaBannerService.AddBannerItem(id, request);
                return Ok(bannerItem);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("/odata/GachaBanners")]
        [EnableQuery]
        public IActionResult GetOData()
        {
            return Ok(_gachaBannerService.GetBannersQueryable());
        }

        [HttpGet("/odata/GachaBannerItems")]
        [EnableQuery]
        public IActionResult GetItemsOData()
        {
            return Ok(_gachaBannerService.GetBannerItemsQueryable());
        }
    }
}
