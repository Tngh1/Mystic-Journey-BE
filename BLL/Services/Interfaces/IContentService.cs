using BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IContentService
    {
        Task<ContentDetailResponseDto?> GetContentById(int id);
        Task<ContentDetailResponseDto?> GetContentBySlug(string slug);
        Task<ContentDetailResponseDto> CreateContentWithBlocksAsync(CreateContentWithBlocksRequestDto request);
        Task<ContentResponseDto> UpdateContent(int id, UpdateContentRequestDto request);
        Task<ContentResponseDto> PublishContent(int id);
        Task<CategoryContentResponseDto> CreateCategory(CreateCategoryContentRequestDto request);
        Task<List<CategoryContentResponseDto>> GetAllCategories(string? search = null, bool? isActive = null);
        Task<PagedResultDto<CategoryContentResponseDto>> GetCategoriesPaged(int page, int pageSize, string? search = null, bool? isActive = null);
        Task<CategoryContentResponseDto> UpdateCategory(int id, CreateCategoryContentRequestDto request);
        Task<BlockContentResponseDto> CreateBlock(CreateBlockContentRequestDto request);
        Task<BlockContentResponseDto> UpdateBlock(int id, UpdateBlockContentRequestDto request);
        Task RemoveBlock(int id);
        Task<PagedResultDto<ContentResponseDto>> GetContentsPaged(int page, int pageSize, string? search, bool? isPublished);
    }
}
