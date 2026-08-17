using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    // Queries the database to retrieve i mailbox repository records.
    public class MailboxRepository : IMailboxRepository
    {
        private readonly MysticJourneyDbContext _context;

        // Initializes a new instance of MailboxRepository with dependencies: context.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public MailboxRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }


        // Queries the database to retrieve get mailbox by id records.
        // Query details: eagerly loads related entity navigation properties; sorts records according to business ordering rules.
        // Returns the matching Mailbox? entity result or default if not found.
        public async Task<Mailbox?> GetMailboxById(int id)
        {
            return await _context.Mailboxes
                .Include(m => m.PlayerProfile)  // Eagerly load related navigation entities to avoid N+1 queries
                .Include(m => m.AttachedItems).ThenInclude(a => a.Item)  // Eagerly load related navigation entities to avoid N+1 queries
                .FirstOrDefaultAsync(m => m.MailboxId == id);  // Fetch single matching record or null if not found
        }

        // Queries the database to retrieve get mailboxes by player id records.
        // Query details: eagerly loads related entity navigation properties; sorts records according to business ordering rules.
        // Returns the matching List<Mailbox entity result or default if not found.
        public async Task<List<Mailbox>> GetMailboxesByPlayerId(int playerProfileId)
        {
            return await _context.Mailboxes
                .Include(m => m.PlayerProfile)  // Eagerly load related navigation entities to avoid N+1 queries
                .Include(m => m.AttachedItems).ThenInclude(a => a.Item)  // Eagerly load related navigation entities to avoid N+1 queries
                .Where(m => m.PlayerProfileId == playerProfileId && !m.IsDeleted)  // Filter records matching the predicate
                .OrderByDescending(m => m.SentAt)  // Sort results newest/highest first
                .ToListAsync();  // Materialize the query into a list from the database
        }

        // Performs database query and transactional persistence workflow for get unread mailboxes by player id.
        // Query details: eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync; sorts records according to business ordering rules.
        // Returns the matching List<Mailbox entity result or default if not found.
        public async Task<List<Mailbox>> GetUnreadMailboxesByPlayerId(int playerProfileId)
        {
            return await _context.Mailboxes
                .Include(m => m.PlayerProfile)  // Eagerly load related navigation entities to avoid N+1 queries
                .Include(m => m.AttachedItems).ThenInclude(a => a.Item)  // Eagerly load related navigation entities to avoid N+1 queries
                .Where(m => m.PlayerProfileId == playerProfileId && !m.IsRead)  // Filter records matching the predicate
                .OrderByDescending(m => m.SentAt)  // Sort results newest/highest first
                .ToListAsync();  // Materialize the query into a list from the database
        }


        // Persists state modifications to the database for create mailbox.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching Mailbox entity result or default if not found.
        public async Task<Mailbox> CreateMailbox(Mailbox mailbox)
        {
            mailbox.SentAt = DateTime.UtcNow;
            await _context.Mailboxes.AddAsync(mailbox);  // Stage new entity for insertion in the next SaveChanges call
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return mailbox;
        }

        // Queries the database to retrieve create bulk mailboxes records.
        // Returns the matching List<Mailbox entity result or default if not found.
        public async Task<List<Mailbox>> CreateBulkMailboxes(List<Mailbox> mailboxes)
        {
            var now = DateTime.UtcNow;
            foreach (var mailbox in mailboxes)
            {
                mailbox.SentAt = now;
            }
            await _context.Mailboxes.AddRangeAsync(mailboxes);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return mailboxes;
        }

        // Persists state modifications to the database for update mailbox.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching Mailbox entity result or default if not found.
        public async Task<Mailbox> UpdateMailbox(Mailbox mailbox)
        {
            _context.Mailboxes.Update(mailbox);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return mailbox;
        }

        // Queries the database to retrieve soft delete mailbox records.
        // Returns the matching Mailbox entity result or default if not found.
        public async Task<Mailbox> SoftDeleteMailbox(int mailboxId)
        {
            var mailbox = await _context.Mailboxes.FindAsync(mailboxId)
                ?? throw new KeyNotFoundException($"Mailbox with id {mailboxId} not found.");
            mailbox.IsDeleted = true;
            mailbox.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return mailbox;
        }


        // Queries the database to retrieve get mailboxes paged records.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties; sorts records according to business ordering rules; applies pagination offset and limit parameters.
        public async Task<(int TotalCount, List<Mailbox> Items)> GetMailboxesPaged(int page, int pageSize, string? search, bool? isRead, bool? isClaimed, string? sortBy = null, string? sortOrder = null)
        {
            var query = _context.Mailboxes
                .Include(m => m.PlayerProfile)  // Eagerly load related navigation entities to avoid N+1 queries
                .Include(m => m.AttachedItems).ThenInclude(a => a.Item)  // Eagerly load related navigation entities to avoid N+1 queries
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking();  // Disable EF Core change tracking for this read-only query

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Title.Contains(search));  // Filter records matching the predicate
            }
            if (isRead.HasValue)
            {
                query = query.Where(x => x.IsRead == isRead.Value);  // Filter records matching the predicate
            }
            if (isClaimed.HasValue)
            {
                query = query.Where(x => x.IsClaimed == isClaimed.Value);  // Filter records matching the predicate
            }

            bool desc = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);
            query = (sortBy?.ToLowerInvariant()) switch
            {
                "title" => desc ? query.OrderByDescending(x => x.Title) : query.OrderBy(x => x.Title),  // Sort results newest/highest first
                "sentat" => desc ? query.OrderByDescending(x => x.SentAt) : query.OrderBy(x => x.SentAt),  // Sort results newest/highest first
                "expiresat" => desc ? query.OrderByDescending(x => x.ExpiredAt) : query.OrderBy(x => x.ExpiredAt),  // Sort results newest/highest first
                "isread" => desc ? query.OrderByDescending(x => x.IsRead) : query.OrderBy(x => x.IsRead),  // Sort results newest/highest first
                "isclaimed" => desc ? query.OrderByDescending(x => x.IsClaimed) : query.OrderBy(x => x.IsClaimed),  // Sort results newest/highest first
                _ => desc ? query.OrderByDescending(x => x.MailboxId) : query.OrderBy(x => x.MailboxId),  // Sort results newest/highest first
            };

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();  // Materialize the query into a list from the database

            return (totalCount, items);
        }

        // Queries the database to retrieve get mailboxes by player id paged records.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties.
        public async Task<(int TotalCount, List<Mailbox> Items)> GetMailboxesByPlayerIdPaged(int playerProfileId, int page, int pageSize)
        {
            var query = _context.Mailboxes
                .Include(m => m.PlayerProfile)  // Eagerly load related navigation entities to avoid N+1 queries
                .Include(m => m.AttachedItems).ThenInclude(a => a.Item)  // Eagerly load related navigation entities to avoid N+1 queries
                .Where(m => m.PlayerProfileId == playerProfileId && !m.IsDeleted)  // Filter records matching the predicate
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking();  // Disable EF Core change tracking for this read-only query

            int totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(m => m.SentAt)  // Sort results newest/highest first
                .Skip((page - 1) * pageSize)  // Apply pagination offset — skip already-seen records
                .Take(pageSize)  // Apply pagination limit — cap result set size
                .ToListAsync();  // Materialize the query into a list from the database

            return (totalCount, items);
        }
    }
}
