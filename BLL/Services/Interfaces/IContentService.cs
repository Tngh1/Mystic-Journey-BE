using BLL.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IContentService
    {
        Task<ContentDetailResponseDto?> GetContentById(int id);
        Task<ContentDetailResponseDto?> GetContentBySlug(string slug);
        Task<ContentResponseDto> CreateContent(CreateContentRequestDto request);
        Task<ContentResponseDto> UpdateContent(int id, UpdateContentRequestDto request);
        Task<ContentResponseDto> PublishContent(int id);
        Task<List<CategoryContentResponseDto>> GetAllCategories();
        Task<CategoryContentResponseDto> CreateCategory(CreateCategoryContentRequestDto request);
        Task<BlockContentResponseDto> CreateBlock(CreateBlockContentRequestDto request);
        Task<BlockContentResponseDto> UpdateBlock(int id, UpdateBlockContentRequestDto request);
        IQueryable<ContentResponseDto> GetContentsQueryable();
        IQueryable<CategoryContentResponseDto> GetCategoriesQueryable();
        IQueryable<BlockContentResponseDto> GetBlocksQueryable();
    }
}
