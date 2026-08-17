using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    // Queries the database to retrieve i chat moderation repository records.
    public class ChatModerationRepository : IChatModerationRepository
    {
        private readonly MysticJourneyDbContext _context;

        // Initializes a new instance of ChatModerationRepository with dependencies: context.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public ChatModerationRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        // Queries the database to retrieve get active penalty records.
        // Query details: uses AsNoTracking() for read-only query optimization; sorts records according to business ordering rules.
        // Returns the matching ChatModerationPenalty? entity result or default if not found.
        public async Task<ChatModerationPenalty?> GetActivePenalty(int playerProfileId, DateTime now)
        {
            return await _context.ChatModerationPenalties
                .Where(x => x.PlayerProfileId == playerProfileId && x.LockedUntil > now)  // Filter records matching the predicate
                .OrderByDescending(x => x.LockedUntil)  // Sort results newest/highest first
                .ThenByDescending(x => x.ChatModerationPenaltyId)
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking()  // Disable EF Core change tracking for this read-only query
                .FirstOrDefaultAsync();  // Fetch single matching record or null if not found
        }

        // Queries the database to retrieve count penalties records.
        // Query details: uses AsNoTracking() for read-only query optimization.
        // Returns the computed numeric count or database ID result.
        public async Task<int> CountPenalties(int playerProfileId)
        {
            return await _context.ChatModerationPenalties
                .CountAsync(x => x.PlayerProfileId == playerProfileId);
        }

        // Queries the database to retrieve get penalty by chat message id records.
        // Query details: uses AsNoTracking() for read-only query optimization.
        // Returns the matching ChatModerationPenalty? entity result or default if not found.
        public async Task<ChatModerationPenalty?> GetPenaltyByChatMessageId(int chatMessageId)
        {
            return await _context.ChatModerationPenalties
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking()  // Disable EF Core change tracking for this read-only query
                .FirstOrDefaultAsync(x => x.ChatMessageId == chatMessageId);  // Fetch single matching record or null if not found
        }

        // Performs database query and transactional persistence workflow for get penalty by world message id.
        // Query details: uses AsNoTracking() for read-only query optimization; commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching ChatModerationPenalty? entity result or default if not found.
        public async Task<ChatModerationPenalty?> GetPenaltyByWorldMessageId(int worldChatMessageId)
        {
            return await _context.ChatModerationPenalties
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking()  // Disable EF Core change tracking for this read-only query
                .FirstOrDefaultAsync(x => x.WorldChatMessageId == worldChatMessageId);  // Fetch single matching record or null if not found
        }

        // Persists state modifications to the database for create.
        // Returns the matching ChatModerationPenalty entity result or default if not found.
        public async Task<ChatModerationPenalty> Create(ChatModerationPenalty penalty)
        {
            await _context.ChatModerationPenalties.AddAsync(penalty);  // Stage new entity for insertion in the next SaveChanges call
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return penalty;
        }
    }
}
