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
    // Queries the database to retrieve i chat message repository records.
    public class ChatMessageRepository : IChatMessageRepository
    {
        private readonly MysticJourneyDbContext _context;

        // Initializes a new instance of ChatMessageRepository with dependencies: context.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public ChatMessageRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        // Load world messages paged using total count, page, and page size; it filters the eligible records, orders the resulting records, and materializes the query results.
        public async Task<(int TotalCount, List<WorldChatMessage> Items)> GetWorldMessagesPaged(
            int page,
            int pageSize)
        {
            page = Math.Max(1, page);
            pageSize = Math.Max(1, pageSize);

            var filtered = _context.WorldChatMessages
                .Where(m => !m.IsHidden)  // Filter records matching the predicate
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking();  // Disable EF Core change tracking for this read-only query

            var totalCount = await filtered.CountAsync();
            var items = await filtered
                .Include(m => m.Sender)  // Eagerly load related navigation entities to avoid N+1 queries
                .OrderByDescending(m => m.SentAt)  // Sort results newest/highest first
                .ThenByDescending(m => m.WorldChatMessageId)
                .Skip((page - 1) * pageSize)  // Apply pagination offset — skip already-seen records
                .Take(pageSize)  // Apply pagination limit — cap result set size
                .ToListAsync();  // Materialize the query into a list from the database

            return (totalCount, items);
        }

        // Performs database query and transactional persistence workflow for get latest world sent at by sender id.
        // Query details: commits entity state changes via EF Core SaveChangesAsync; sorts records according to business ordering rules.
        // Returns the matching DateTime? entity result or default if not found.
        public async Task<DateTime?> GetLatestWorldSentAtBySenderId(int senderId)
        {
            return await _context.WorldChatMessages
                .Where(m => m.SenderId == senderId)  // Filter records matching the predicate
                .OrderByDescending(m => m.SentAt)  // Sort results newest/highest first
                .Select(m => (DateTime?)m.SentAt)
                .FirstOrDefaultAsync();  // Fetch single matching record or null if not found
        }

        // Performs database query and transactional persistence workflow for create world message.
        // Query details: eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching WorldChatMessage entity result or default if not found.
        public async Task<WorldChatMessage> CreateWorldMessage(WorldChatMessage message)
        {
            await _context.WorldChatMessages.AddAsync(message);  // Stage new entity for insertion in the next SaveChanges call
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database

            if (message.Sender == null)
                await _context.Entry(message).Reference(m => m.Sender).LoadAsync();

            return message;
        }

        // Performs database query and transactional persistence workflow for get world message by id.
        // Query details: eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching WorldChatMessage? entity result or default if not found.
        public async Task<WorldChatMessage?> GetWorldMessageById(int worldChatMessageId)
        {
            return await _context.WorldChatMessages
                .Include(m => m.Sender)  // Eagerly load related navigation entities to avoid N+1 queries
                .FirstOrDefaultAsync(m =>  // Fetch single matching record or null if not found
                    m.WorldChatMessageId == worldChatMessageId &&
                    !m.IsHidden);
        }

        // Performs database query and transactional persistence workflow for update world message.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync; sorts records according to business ordering rules; applies pagination offset and limit parameters.
        // Returns the matching WorldChatMessage entity result or default if not found.
        public async Task<WorldChatMessage> UpdateWorldMessage(WorldChatMessage message)
        {
            _context.WorldChatMessages.Update(message);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database

            if (message.Sender == null)
                await _context.Entry(message).Reference(m => m.Sender).LoadAsync();

            return message;
        }

        // Load conversation paged using total count, player profile id, other player profile id, and page; it filters the eligible records, orders the resulting records, and materializes the query results.
        public async Task<(int TotalCount, List<ChatMessage> Items)> GetConversationPaged(
            int playerProfileId,
            int otherPlayerProfileId,
            int page,
            int pageSize)
        {
            page = Math.Max(1, page);
            pageSize = Math.Max(1, pageSize);

            var filtered = _context.ChatMessages
                .Where(m =>  // Filter records matching the predicate
                    !m.IsHidden &&
                    ((m.SenderId == playerProfileId && m.RecipientId == otherPlayerProfileId) ||
                     (m.SenderId == otherPlayerProfileId && m.RecipientId == playerProfileId)))
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking();  // Disable EF Core change tracking for this read-only query

            var totalCount = await filtered.CountAsync();
            var items = await filtered
                .Include(m => m.Sender)  // Eagerly load related navigation entities to avoid N+1 queries
                .Include(m => m.Recipient)  // Eagerly load related navigation entities to avoid N+1 queries
                .OrderByDescending(m => m.SentAt)  // Sort results newest/highest first
                .ThenByDescending(m => m.ChatMessageId)
                .Skip((page - 1) * pageSize)  // Apply pagination offset — skip already-seen records
                .Take(pageSize)  // Apply pagination limit — cap result set size
                .ToListAsync();  // Materialize the query into a list from the database

            return (totalCount, items);
        }

        // Performs database query and transactional persistence workflow for get latest sent at by sender id.
        // Query details: commits entity state changes via EF Core SaveChangesAsync; sorts records according to business ordering rules.
        // Returns the matching DateTime? entity result or default if not found.
        public async Task<DateTime?> GetLatestSentAtBySenderId(int senderId)
        {
            return await _context.ChatMessages
                .Where(m => m.SenderId == senderId)  // Filter records matching the predicate
                .OrderByDescending(m => m.SentAt)  // Sort results newest/highest first
                .Select(m => (DateTime?)m.SentAt)
                .FirstOrDefaultAsync();  // Fetch single matching record or null if not found
        }

        // Performs database query and transactional persistence workflow for create.
        // Query details: eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching ChatMessage entity result or default if not found.
        public async Task<ChatMessage> Create(ChatMessage message)
        {
            await _context.ChatMessages.AddAsync(message);  // Stage new entity for insertion in the next SaveChanges call
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database

            if (message.Sender == null)
                await _context.Entry(message).Reference(m => m.Sender).LoadAsync();
            if (message.Recipient == null)
                await _context.Entry(message).Reference(m => m.Recipient).LoadAsync();

            return message;
        }

        // Performs database query and transactional persistence workflow for get message by id.
        // Query details: eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching ChatMessage? entity result or default if not found.
        public async Task<ChatMessage?> GetMessageById(int chatMessageId)
        {
            return await _context.ChatMessages
                .Include(m => m.Sender)  // Eagerly load related navigation entities to avoid N+1 queries
                .Include(m => m.Recipient)  // Eagerly load related navigation entities to avoid N+1 queries
                .FirstOrDefaultAsync(m =>  // Fetch single matching record or null if not found
                    m.ChatMessageId == chatMessageId &&
                    !m.IsHidden);
        }

        // Per-frame update loop for ChatMessageRepository.
        // Handles real-time input polling, smooth interpolations, cooldown timers, and UI updates.
        public async Task<ChatMessage> Update(ChatMessage message)
        {
            _context.ChatMessages.Update(message);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database

            if (message.Sender == null)
                await _context.Entry(message).Reference(m => m.Sender).LoadAsync();
            if (message.Recipient == null)
                await _context.Entry(message).Reference(m => m.Recipient).LoadAsync();

            return message;
        }
        // Queries the database to retrieve delete conversation records.
        // Returns the computed numeric count or database ID result.
        public async Task<int> DeleteConversation(int firstPlayerProfileId, int secondPlayerProfileId)
        {
            return await _context.ChatMessages
                .Where(m =>  // Filter records matching the predicate
                    (m.SenderId == firstPlayerProfileId && m.RecipientId == secondPlayerProfileId) ||
                    (m.SenderId == secondPlayerProfileId && m.RecipientId == firstPlayerProfileId))
                // Apply this bulk change directly in the database without loading every affected entity.
                .ExecuteDeleteAsync();
        }
    }
}
