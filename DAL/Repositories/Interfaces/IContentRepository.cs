using DAL.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IContentRepository
    {
        Task<Content?> GetContentById(int id);
        Task<Content?> GetContentByIdWithBlocks(int id);
        Task<Content?> GetContentBySlug(string slug);
        Task<List<Content>> GetAllContents();
        Task<List<Content>> GetPublishedContents();
        Task<Content> CreateContent(Content content);
        Task<Content> UpdateContent(Content content);
        Task<CategoryContent?> GetCategoryById(int id);
        Task<List<CategoryContent>> GetAllCategories();
        Task<CategoryContent> CreateCategory(CategoryContent category);
        Task<BlockContent?> GetBlockById(int id);
        Task<List<BlockContent>> GetBlocksByContentId(int contentId);
        Task<BlockContent> CreateBlock(BlockContent block);
        Task<BlockContent> UpdateBlock(BlockContent block);
        Task<(int TotalCount, List<Content> Items)> GetContentsPaged(int page, int pageSize, string? search, bool? isPublished, bool? isActive);
        Task<(int TotalCount, List<CategoryContent> Items)> GetCategoriesPaged(int page, int pageSize);
        Task<(int TotalCount, List<BlockContent> Items)> GetBlocksPaged(int page, int pageSize);
    }
}
