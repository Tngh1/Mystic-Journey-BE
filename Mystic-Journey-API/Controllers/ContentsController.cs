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
    public class ContentsController : ControllerBase
    {
        private readonly IContentService _contentService;

        // Initializes a new instance of ContentsController with dependencies: contentService.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public ContentsController(IContentService contentService)
        {
            _contentService = contentService;
        }


        // ─── Guest APIs ───────────────────────────────────────────────────────
        [AllowAnonymous]
        [HttpGet("{id}")]
        // Retrieves published or draft CMS content article by ID.
        public async Task<IActionResult> GetById(int id)
        {
            var content = await _contentService.GetContentById(id); // Look up article and nested content blocks
            if (content == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Content with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            return Ok(new ApiResponse<ContentDetailResponseDto> { Success = true, Data = content });
        }

        [AllowAnonymous]
        [HttpGet("slug/{slug}")]
        // Retrieves public website news or patch notes article by URL-friendly slug.
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var content = await _contentService.GetContentBySlug(slug); // Query article using unique slug identifier
            if (content == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Content with slug '{slug}' not found.", ErrorCode = ErrorCodes.NotFound });

            return Ok(new ApiResponse<ContentDetailResponseDto> { Success = true, Data = content });
        }

        [AllowAnonymous]
        [HttpGet]
        // Retrieves paginated list of CMS announcements, blogs, and patch notes.
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] bool? isPublished = null, [FromQuery] int? categoryId = null)
        {
            var result = await _contentService.GetContentsPaged(page, pageSize, search, isPublished, categoryId); // Query articles with category and publication filters
            return Ok(new ApiResponse<PagedResultDto<ContentResponseDto>> { Success = true, Data = result });
        }

        [AllowAnonymous]
        [HttpGet("categories")]
        // Retrieves list of article categories (Announcements, Events, Patch Notes, Guides).
        public async Task<IActionResult> GetCategories([FromQuery] int? page = null, [FromQuery] int? pageSize = null, [FromQuery] string? search = null, [FromQuery] bool? isActive = null)
        {
            if (page.HasValue && pageSize.HasValue)
            {
                var pagedResult = await _contentService.GetCategoriesPaged(page.Value, pageSize.Value, search, isActive); // Paginated category query
                return Ok(new ApiResponse<PagedResultDto<CategoryContentResponseDto>> { Success = true, Data = pagedResult });
            }

            var allResult = await _contentService.GetAllCategories(search, isActive); // All categories query
            return Ok(new ApiResponse<List<CategoryContentResponseDto>> { Success = true, Data = allResult });
        }


        // ─── Admin APIs ───────────────────────────────────────────────────────
        [Authorize(Roles = "Admin")]
        [HttpPost("with-blocks")]
        // Creates a rich CMS content post with associated layout blocks (text, image, embed, quote).
        public async Task<IActionResult> CreateWithBlocks([FromBody] CreateContentWithBlocksRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var content = await _contentService.CreateContentWithBlocksAsync(request); // Save post metadata and child content block records atomically
            return Ok(new ApiResponse<ContentDetailResponseDto> { Success = true, Data = content });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        // Updates post title, slug, summary, category, or thumbnail.
        public async Task<IActionResult> Update(int id, [FromBody] UpdateContentRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var content = await _contentService.UpdateContent(id, request); // Save updated article metadata
            return Ok(new ApiResponse<ContentResponseDto> { Success = true, Data = content });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/publish")]
        // Toggles published status and sets publish timestamp for public visibility.
        public async Task<IActionResult> Publish(int id)
        {
            var content = await _contentService.PublishContent(id); // Set IsPublished = true and update publication date
            return Ok(new ApiResponse<ContentResponseDto> { Success = true, Data = content });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("categories")]
        // Creates a new content category.
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryContentRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var category = await _contentService.CreateCategory(request); // Insert category row
            return Ok(new ApiResponse<CategoryContentResponseDto> { Success = true, Data = category });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("categories/{id}")]
        // Updates a content category's name and description.
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CreateCategoryContentRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var category = await _contentService.UpdateCategory(id, request); // Save category updates
            return Ok(new ApiResponse<CategoryContentResponseDto> { Success = true, Data = category });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("blocks")]
        // Executes create block operation.
        public async Task<IActionResult> CreateBlock([FromBody] CreateBlockContentRequestDto request)
        {
            if (!ModelState.IsValid)  // Reject request immediately if any DTO validation annotation fails
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });  // Return HTTP 400 with validation error details

            var block = await _contentService.CreateBlock(request);
            return Ok(new ApiResponse<BlockContentResponseDto> { Success = true, Data = block });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("blocks/{id}")]
        // Executes update block operation.
        public async Task<IActionResult> UpdateBlock(int id, [FromBody] UpdateBlockContentRequestDto request)
        {
            if (!ModelState.IsValid)  // Reject request immediately if any DTO validation annotation fails
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });  // Return HTTP 400 with validation error details

            var block = await _contentService.UpdateBlock(id, request);
            return Ok(new ApiResponse<BlockContentResponseDto> { Success = true, Data = block });  // Return HTTP 200 with standard ApiResponse envelope
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("blocks/{id}")]
        // Executes remove block operation.
        public async Task<IActionResult> RemoveBlock(int id)
        {
            await _contentService.RemoveBlock(id);
            return Ok(new ApiResponse<object> { Success = true, Message = "Block removed successfully." });  // Return HTTP 200 with standard ApiResponse envelope
        }
    }
}
