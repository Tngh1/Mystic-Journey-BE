using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    // Queries the database to retrieve i content repository records.
    public class ContentRepository : IContentRepository
    {
        private readonly MysticJourneyDbContext _context;

        // Initializes a new instance of ContentRepository with dependencies: context.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public ContentRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        // Queries the database to retrieve get content by id records.
        // Query details: eagerly loads related entity navigation properties.
        // Returns the matching Content? entity result or default if not found.
        public async Task<Content?> GetContentById(int id)
        {
            return await _context.Contents
                .FirstOrDefaultAsync(c => c.ContentId == id);  // Fetch single matching record or null if not found
        }

        // Queries the database to retrieve get content by id with blocks records.
        // Query details: eagerly loads related entity navigation properties.
        // Returns the matching Content? entity result or default if not found.
        public async Task<Content?> GetContentByIdWithBlocks(int id)
        {
            return await _context.Contents
                .Include(c => c.BlockContents)  // Eagerly load related navigation entities to avoid N+1 queries
                .Include(c => c.CategoryContent)  // Eagerly load related navigation entities to avoid N+1 queries
                .FirstOrDefaultAsync(c => c.ContentId == id);  // Fetch single matching record or null if not found
        }

        // Performs database query and transactional persistence workflow for get content by slug.
        // Query details: eagerly loads related entity navigation properties; executes within an atomic database transaction; commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching Content? entity result or default if not found.
        public async Task<Content?> GetContentBySlug(string slug)
        {
            return await _context.Contents
                .Include(c => c.BlockContents)  // Eagerly load related navigation entities to avoid N+1 queries
                .Include(c => c.CategoryContent)  // Eagerly load related navigation entities to avoid N+1 queries
                .FirstOrDefaultAsync(c => c.Slug.ToLower() == slug.ToLower());  // Fetch single matching record or null if not found
        }

        // Persists state modifications to the database for create content with blocks async.
        // Query details: executes within an atomic database transaction; commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching Content entity result or default if not found.
        public async Task<Content> CreateContentWithBlocksAsync(Content content, IList<BlockContent> blocks)
        {
            // Keep the following dependent database writes in one transaction so a failure cannot persist partial state.
            await using var transaction = await _context.Database.BeginTransactionAsync();  // Open serializable transaction — prevents race conditions on concurrent purchases
            try
            {
                await _context.Contents.AddAsync(content);  // Stage new entity for insertion in the next SaveChanges call
                await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database

                if (blocks != null && blocks.Count > 0)
                {
                    foreach (var block in blocks)
                    {
                        block.ContentId = content.ContentId;
                    }

                    await _context.BlockContents.AddRangeAsync(blocks);
                    await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
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

        // Performs database query and transactional persistence workflow for update content.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching Content entity result or default if not found.
        public async Task<Content> UpdateContent(Content content)
        {
            _context.Contents.Update(content);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return content;
        }


        // Queries the database to retrieve get category by id records.
        // Query details: uses AsNoTracking() for read-only query optimization.
        // Returns the matching CategoryContent? entity result or default if not found.
        public async Task<CategoryContent?> GetCategoryById(int id)
        {
            return await _context.CategoryContents
                .FirstOrDefaultAsync(c => c.CategoryContentId == id);  // Fetch single matching record or null if not found
        }

        // Queries the database to retrieve get all categories records.
        // Query details: uses AsNoTracking() for read-only query optimization; applies pagination offset and limit parameters.
        // Returns the matching List<CategoryContent entity result or default if not found.
        public async Task<List<CategoryContent>> GetAllCategories(string? search, bool? isActive)
        {
            // Execute this query without change tracking because the returned entities are read-only.
            var query = _context.CategoryContents.AsNoTracking();  // Disable EF Core change tracking for this read-only query
            if (!string.IsNullOrEmpty(search))
                query = query.Where(c => c.Name.Contains(search) || c.Slug.Contains(search));  // Filter records matching the predicate
            if (isActive.HasValue)
                query = query.Where(c => c.IsActive == isActive.Value);  // Filter records matching the predicate
            return await query.ToListAsync();  // Materialize the query into a list from the database
        }

        // Performs database query and transactional persistence workflow for get categories paged.
        // Query details: uses AsNoTracking() for read-only query optimization; commits entity state changes via EF Core SaveChangesAsync; applies pagination offset and limit parameters.
        public async Task<(int TotalCount, List<CategoryContent> Items)> GetCategoriesPaged(int page, int pageSize, string? search, bool? isActive)
        {
            // Execute this query without change tracking because the returned entities are read-only.
            var query = _context.CategoryContents.AsNoTracking();  // Disable EF Core change tracking for this read-only query
            if (!string.IsNullOrEmpty(search))
                query = query.Where(c => c.Name.Contains(search) || c.Slug.Contains(search));  // Filter records matching the predicate
            if (isActive.HasValue)
                query = query.Where(c => c.IsActive == isActive.Value);  // Filter records matching the predicate

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();  // Materialize the query into a list from the database
            return (totalCount, items);
        }

        // Persists state modifications to the database for create category.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching CategoryContent entity result or default if not found.
        public async Task<CategoryContent> CreateCategory(CategoryContent category)
        {
            await _context.CategoryContents.AddAsync(category);  // Stage new entity for insertion in the next SaveChanges call
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return category;
        }

        // Performs database query and transactional persistence workflow for update category.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching CategoryContent entity result or default if not found.
        public async Task<CategoryContent> UpdateCategory(CategoryContent category)
        {
            _context.CategoryContents.Update(category);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return category;
        }

        // Performs database query and transactional persistence workflow for get block by id.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching BlockContent? entity result or default if not found.
        public async Task<BlockContent?> GetBlockById(int id)
        {
            return await _context.BlockContents
                .FirstOrDefaultAsync(b => b.Id == id);  // Fetch single matching record or null if not found
        }

        // Persists state modifications to the database for create block.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching BlockContent entity result or default if not found.
        public async Task<BlockContent> CreateBlock(BlockContent block)
        {
            await _context.BlockContents.AddAsync(block);  // Stage new entity for insertion in the next SaveChanges call
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return block;
        }

        // Persists state modifications to the database for update block.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching BlockContent entity result or default if not found.
        public async Task<BlockContent> UpdateBlock(BlockContent block)
        {
            _context.BlockContents.Update(block);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return block;
        }

        // Persists state modifications to the database for remove block.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        public async Task RemoveBlock(int id)
        {
            var block = await _context.BlockContents.FindAsync(id);
            if (block != null)  // Entity exists — proceed with conditional branch
            {
                _context.BlockContents.Remove(block);  // Mark entity for deletion in the next SaveChanges call
                await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            }
        }

        // Queries the database to retrieve unpublish by category id async records.
        // Returns the computed numeric count or database ID result.
        public async Task<int> UnpublishByCategoryIdAsync(int categoryId)
        {
            var publishedContents = await _context.Contents
                .Where(c => c.CategoryContentId == categoryId && c.IsPublished)  // Filter records matching the predicate
                .ToListAsync();  // Materialize the query into a list from the database

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

            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return publishedContents.Count;
        }

        // Queries the database to retrieve get contents paged records.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties; applies pagination offset and limit parameters.
        public async Task<(int TotalCount, List<Content> Items)> GetContentsPaged(int page, int pageSize, string? search, bool? isPublished, int? categoryId = null)
        {
            var query = _context.Contents
                .Include(c => c.CategoryContent)  // Eagerly load related navigation entities to avoid N+1 queries
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking();  // Disable EF Core change tracking for this read-only query

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Title.Contains(search) || x.Slug.Contains(search));  // Filter records matching the predicate
            }
            if (isPublished.HasValue)
            {
                query = query.Where(x => x.IsPublished == isPublished.Value);  // Filter records matching the predicate
            }
            if (categoryId.HasValue)
            {
                query = query.Where(x => x.CategoryContentId == categoryId.Value);  // Filter records matching the predicate
            }

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();  // Materialize the query into a list from the database

            return (totalCount, items);
        }
    }
}
