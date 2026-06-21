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

        public async Task<Content> CreateContentWithBlocksAsync(Content content, IList<BlockContent> blocks)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Contents.AddAsync(content);
                await _context.SaveChangesAsync();

                if (blocks != null && blocks.Count > 0)
                {
                    foreach (var block in blocks)
                    {
                        block.ContentId = content.ContentId;
                    }

                    await _context.BlockContents.AddRangeAsync(blocks);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return content;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
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

        public async Task<List<CategoryContent>> GetAllCategories(string? search, bool? isActive)
        {
            var query = _context.CategoryContents.AsNoTracking();
            if (!string.IsNullOrEmpty(search))
                query = query.Where(c => c.Name.Contains(search) || c.Slug.Contains(search));
            if (isActive.HasValue)
                query = query.Where(c => c.IsActive == isActive.Value);
            return await query.ToListAsync();
        }

        public async Task<(int TotalCount, List<CategoryContent> Items)> GetCategoriesPaged(int page, int pageSize, string? search, bool? isActive)
        {
            var query = _context.CategoryContents.AsNoTracking();
            if (!string.IsNullOrEmpty(search))
                query = query.Where(c => c.Name.Contains(search) || c.Slug.Contains(search));
            if (isActive.HasValue)
                query = query.Where(c => c.IsActive == isActive.Value);

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (totalCount, items);
        }

        public async Task<CategoryContent> CreateCategory(CategoryContent category)
        {
            await _context.CategoryContents.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<CategoryContent> UpdateCategory(CategoryContent category)
        {
            _context.CategoryContents.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<BlockContent?> GetBlockById(int id)
        {
            return await _context.BlockContents
                .FirstOrDefaultAsync(b => b.BlockContentId == id);
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

        public async Task RemoveBlock(int id)
        {
            var block = await _context.BlockContents.FindAsync(id);
            if (block != null)
            {
                _context.BlockContents.Remove(block);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> UnpublishByCategoryIdAsync(int categoryId)
        {
            var publishedContents = await _context.Contents
                .Where(c => c.CategoryContentId == categoryId && c.IsPublished)
                .ToListAsync();

            if (publishedContents.Count == 0)
            {
                return 0;
            }

            var now = DateTime.UtcNow;
            foreach (var content in publishedContents)
            {
                content.IsPublished = false;
                content.UpdatedAt = now;
            }

            await _context.SaveChangesAsync();
            return publishedContents.Count;
        }

        public async Task<(int TotalCount, List<Content> Items)> GetContentsPaged(int page, int pageSize, string? search, bool? isPublished)
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

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (totalCount, items);
        }
    }
}
