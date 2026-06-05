using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

        public async Task<(int TotalCount, List<Content> Items)> GetContentsPaged(int page, int pageSize, string? search, bool? isPublished, bool? isActive)
        {
            var query = _context.Contents
                .Include(c => c.CategoryContent)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Title.Contains(search) || x.Slug.Contains(search));
            }
            if (isPublished.HasValue)
            {
                query = query.Where(x => x.IsPublished == isPublished.Value);
            }
            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);
            }

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (totalCount, items);
        }

        public async Task<(int TotalCount, List<CategoryContent> Items)> GetCategoriesPaged(int page, int pageSize)
        {
            var query = _context.CategoryContents.AsNoTracking();
            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (totalCount, items);
        }

        public async Task<(int TotalCount, List<BlockContent> Items)> GetBlocksPaged(int page, int pageSize)
        {
            var query = _context.BlockContents.AsNoTracking();
            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (totalCount, items);
        }
    }
}
