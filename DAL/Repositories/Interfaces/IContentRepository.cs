using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IContentRepository
    {
        Task<Content?> GetContentById(int id);
        Task<Content?> GetContentByIdWithBlocks(int id);
        Task<Content?> GetContentBySlug(string slug);
        Task<Content> UpdateContent(Content content);
        Task<Content> CreateContentWithBlocksAsync(Content content, IList<BlockContent> blocks);

        Task<CategoryContent?> GetCategoryById(int id);
        Task<List<CategoryContent>> GetAllCategories(string? search, bool? isActive);
        Task<(int TotalCount, List<CategoryContent> Items)> GetCategoriesPaged(int page, int pageSize, string? search, bool? isActive);
        Task<CategoryContent> CreateCategory(CategoryContent category);
        Task<CategoryContent> UpdateCategory(CategoryContent category);
        Task<BlockContent?> GetBlockById(int id);
        Task<BlockContent> CreateBlock(BlockContent block);
        Task<BlockContent> UpdateBlock(BlockContent block);
        Task RemoveBlock(int id);
        Task<int> UnpublishByCategoryIdAsync(int categoryId);
        Task<(int TotalCount, List<Content> Items)> GetContentsPaged(int page, int pageSize, string? search, bool? isPublished);
    }
}
