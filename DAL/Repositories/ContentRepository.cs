using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class ContentRepository : IContentRepository
    {
        private readonly MysticJourneyDbContext _context;

        public ContentRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<Content?> GetContentById(int id)
        {
            return await _context.Contents
                .FirstOrDefaultAsync(c => c.ContentId == id);
        }

        public async Task<Content?> GetContentByIdWithBlocks(int id)
        {
            return await _context.Contents
                .Include(c => c.BlockContents)
                .FirstOrDefaultAsync(c => c.ContentId == id);
        }

        public async Task<Content?> GetContentBySlug(string slug)
        {
            return await _context.Contents
                .Include(c => c.BlockContents)
                .Include(c => c.CategoryContent)
                .FirstOrDefaultAsync(c => c.Slug.ToLower() == slug.ToLower());
        }

        public async Task<List<Content>> GetAllContents()
        {
            return await _context.Contents.ToListAsync();
        }

        public async Task<List<Content>> GetPublishedContents()
        {
            return await _context.Contents
                .Include(c => c.CategoryContent)
                .Where(c => c.IsPublished)
                .ToListAsync();
        }

        public async Task<Content> CreateContent(Content content)
        {
            await _context.Contents.AddAsync(content);
            await _context.SaveChangesAsync();
            return content;
        }

        public async Task<Content> UpdateContent(Content content)
        {
_context.Contents.Update(content);
            await _context.SaveChangesAsync();
            return content;
        }

        public async Task DeleteContent(int id)
        {
            var content = await GetContentById(id);
            if (content != null)
            {
                _context.Contents.Remove(content);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<CategoryContent?> GetCategoryById(int id)
        {
            return await _context.CategoryContents
                .FirstOrDefaultAsync(c => c.CategoryContentId == id);
        }

        public async Task<List<CategoryContent>> GetAllCategories()
        {
            return await _context.CategoryContents.ToListAsync();
        }

        public async Task<CategoryContent> CreateCategory(CategoryContent category)
        {
            await _context.CategoryContents.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<BlockContent?> GetBlockById(int id)
        {
            return await _context.BlockContents
                .FirstOrDefaultAsync(b => b.BlockContentId == id);
        }

        public async Task<List<BlockContent>> GetBlocksByContentId(int contentId)
        {
            return await _context.BlockContents
                .Where(b => b.ContentId == contentId)
                .OrderBy(b => b.SortOrder)
                .ToListAsync();
        }

        public async Task<BlockContent> CreateBlock(BlockContent block)
        {
            await _context.BlockContents.AddAsync(block);
            await _context.SaveChangesAsync();
            return block;
        }

        public async Task<BlockContent> UpdateBlock(BlockContent block)
        {
            _context.BlockContents.Update(block);
            await _context.SaveChangesAsync();
            return block;
        }

        public IQueryable<Content> GetContentsQueryable()
        {
            return _context.Contents
                .Include(c => c.CategoryContent)
                .AsNoTracking();
        }

        public IQueryable<CategoryContent> GetCategoriesQueryable()
        {
            return _context.CategoryContents.AsNoTracking();
        }

        public IQueryable<BlockContent> GetBlocksQueryable()
        {
            return _context.BlockContents.AsNoTracking();
        }
    }
}
