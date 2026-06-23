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
    [ApiController]
    public class ContentsController : ControllerBase
    {
        private readonly IContentService _contentService;

        public ContentsController(IContentService contentService)
        {
            _contentService = contentService;
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var content = await _contentService.GetContentById(id);
            if (content == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Content with id {id} not found.", ErrorCode = ErrorCodes.NotFound });

            return Ok(new ApiResponse<ContentDetailResponseDto> { Success = true, Data = content });
        }

        [AllowAnonymous]
        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var content = await _contentService.GetContentBySlug(slug);
            if (content == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Content with slug '{slug}' not found.", ErrorCode = ErrorCodes.NotFound });

            return Ok(new ApiResponse<ContentDetailResponseDto> { Success = true, Data = content });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("with-blocks")]
        public async Task<IActionResult> CreateWithBlocks([FromBody] CreateContentWithBlocksRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var content = await _contentService.CreateContentWithBlocksAsync(request);
            return Ok(new ApiResponse<ContentDetailResponseDto> { Success = true, Data = content });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateContentRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var content = await _contentService.UpdateContent(id, request);
            return Ok(new ApiResponse<ContentResponseDto> { Success = true, Data = content });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("{id}/publish")]
        public async Task<IActionResult> Publish(int id)
        {
            var content = await _contentService.PublishContent(id);
            return Ok(new ApiResponse<ContentResponseDto> { Success = true, Data = content });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryContentRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var category = await _contentService.CreateCategory(request);
            return Ok(new ApiResponse<CategoryContentResponseDto> { Success = true, Data = category });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories([FromQuery] int? page = null, [FromQuery] int? pageSize = null, [FromQuery] string? search = null, [FromQuery] bool? isActive = null)
        {
            if (page.HasValue && pageSize.HasValue)
            {
                var pagedResult = await _contentService.GetCategoriesPaged(page.Value, pageSize.Value, search, isActive);
                return Ok(new ApiResponse<PagedResultDto<CategoryContentResponseDto>> { Success = true, Data = pagedResult });
            }

            var allResult = await _contentService.GetAllCategories(search, isActive);
            return Ok(new ApiResponse<List<CategoryContentResponseDto>> { Success = true, Data = allResult });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CreateCategoryContentRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var category = await _contentService.UpdateCategory(id, request);
            return Ok(new ApiResponse<CategoryContentResponseDto> { Success = true, Data = category });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost("blocks")]
        public async Task<IActionResult> CreateBlock([FromBody] CreateBlockContentRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var block = await _contentService.CreateBlock(request);
            return Ok(new ApiResponse<BlockContentResponseDto> { Success = true, Data = block });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("blocks/{id}")]
        public async Task<IActionResult> UpdateBlock(int id, [FromBody] UpdateBlockContentRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Validation failed.", ErrorCode = ErrorCodes.ValidationError });

            var block = await _contentService.UpdateBlock(id, request);
            return Ok(new ApiResponse<BlockContentResponseDto> { Success = true, Data = block });
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpDelete("blocks/{id}")]
        public async Task<IActionResult> RemoveBlock(int id)
        {
            await _contentService.RemoveBlock(id);
            return Ok(new ApiResponse<object> { Success = true, Message = "Block removed successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] bool? isPublished = null)
        {
            var result = await _contentService.GetContentsPaged(page, pageSize, search, isPublished);
            return Ok(new ApiResponse<PagedResultDto<ContentResponseDto>> { Success = true, Data = result });
        }
    }
}
