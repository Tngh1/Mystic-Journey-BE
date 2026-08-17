using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    // Queries the database to retrieve i friend repository records.
    public class FriendRepository : IFriendRepository
    {
        private readonly MysticJourneyDbContext _context;

        // Initializes a new instance of FriendRepository with dependencies: context.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public FriendRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        // Queries the database to retrieve get friends records.
        // Returns the matching List<PlayerProfile entity result or default if not found.
        public async Task<List<PlayerProfile>> GetFriends(int playerProfileId)
        {
            var rawFriends = await GetFriendListRaw(playerProfileId);

            var profiles = new List<PlayerProfile>();
            foreach (var f in rawFriends)
            {
                var profile = f.RequesterId == playerProfileId ? f.Addressee : f.Requester;
                if (profile != null) profiles.Add(profile);  // Entity exists — proceed with conditional branch
            }
            return profiles;
        }
        // Queries the database to retrieve get friend list raw records.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties.
        // Returns the matching List<Friend entity result or default if not found.
        public async Task<List<Friend>> GetFriendListRaw(int playerProfileId)
        {
            return await _context.Friends
                .Include(f => f.Requester)  // Eagerly load related navigation entities to avoid N+1 queries
                .Include(f => f.Addressee)  // Eagerly load related navigation entities to avoid N+1 queries
                .Where(f => (f.RequesterId == playerProfileId || f.AddresseeId == playerProfileId) && f.Status == "Accepted")  // Filter records matching the predicate
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking()  // Disable EF Core change tracking for this read-only query
                .ToListAsync();  // Materialize the query into a list from the database
        }

        // Queries the database to retrieve count friends records.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties.
        // Returns the computed numeric count or database ID result.
        public async Task<int> CountFriends(int playerProfileId)
        {
            return await _context.Friends
                .CountAsync(f =>
                    (f.RequesterId == playerProfileId || f.AddresseeId == playerProfileId) &&
                    f.Status == "Accepted");
        }

        // Queries the database to retrieve get friend requests records.
        // Query details: uses AsNoTracking() for read-only query optimization; eagerly loads related entity navigation properties.
        // Returns the matching List<Friend entity result or default if not found.
        public async Task<List<Friend>> GetFriendRequests(int playerProfileId)
        {
            return await _context.Friends
                .Include(f => f.Requester)  // Eagerly load related navigation entities to avoid N+1 queries
                .Where(f => f.AddresseeId == playerProfileId && f.Status == "Pending")  // Filter records matching the predicate
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking()  // Disable EF Core change tracking for this read-only query
                .ToListAsync();  // Materialize the query into a list from the database
        }

        // Queries the database to retrieve get friendship records.
        // Query details: uses AsNoTracking() for read-only query optimization.
        // Returns the matching Friend? entity result or default if not found.
        public async Task<Friend?> GetFriendship(int id1, int id2)
        {
            return await _context.Friends
                .FirstOrDefaultAsync(f =>  // Fetch single matching record or null if not found
                    (f.RequesterId == id1 && f.AddresseeId == id2) ||
                    (f.RequesterId == id2 && f.AddresseeId == id1));
        }

        // Queries the database to retrieve get friendships with records.
        // Query details: uses AsNoTracking() for read-only query optimization.
        // Returns the matching List<Friend entity result or default if not found.
        public async Task<List<Friend>> GetFriendshipsWith(int playerProfileId, List<int> otherIds)
        {
            return await _context.Friends
                .Where(f =>  // Filter records matching the predicate
                    (f.RequesterId == playerProfileId && otherIds.Contains(f.AddresseeId)) ||
                    (f.AddresseeId == playerProfileId && otherIds.Contains(f.RequesterId)))
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking()  // Disable EF Core change tracking for this read-only query
                .ToListAsync();  // Materialize the query into a list from the database
        }

        // Performs database query and transactional persistence workflow for get friend blocks with.
        // Query details: uses AsNoTracking() for read-only query optimization; commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching List<FriendBlock entity result or default if not found.
        public async Task<List<FriendBlock>> GetFriendBlocksWith(int blockerId, List<int> blockedIds)
        {
            return await _context.FriendBlocks
                .Where(fb => fb.BlockerId == blockerId && blockedIds.Contains(fb.BlockedId))  // Filter records matching the predicate
                // Execute this query without change tracking because the returned entities are read-only.
                .AsNoTracking()  // Disable EF Core change tracking for this read-only query
                .ToListAsync();  // Materialize the query into a list from the database
        }

        // Persists state modifications to the database for add friend.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching Friend entity result or default if not found.
        public async Task<Friend> AddFriend(Friend friend)
        {
            _context.Friends.Add(friend);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
            return friend;
        }

        // Persists state modifications to the database for update friend.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        public async Task UpdateFriend(Friend friend)
        {
            _context.Friends.Update(friend);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
        }

        // Performs database query and transactional persistence workflow for remove friend.
        // Query details: eagerly loads related entity navigation properties; commits entity state changes via EF Core SaveChangesAsync.
        public async Task RemoveFriend(Friend friend)
        {
            _context.Friends.Remove(friend);  // Mark entity for deletion in the next SaveChanges call
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
        }

        // Queries the database to retrieve get friend blocks records.
        // Query details: eagerly loads related entity navigation properties.
        // Returns the matching List<FriendBlock entity result or default if not found.
        public async Task<List<FriendBlock>> GetFriendBlocks(int playerProfileId)
        {
            return await _context.FriendBlocks
                .Include(fb => fb.Blocked)  // Eagerly load related navigation entities to avoid N+1 queries
                .Where(fb => fb.BlockerId == playerProfileId)  // Filter records matching the predicate
                .ToListAsync();  // Materialize the query into a list from the database
        }

        // Performs database query and transactional persistence workflow for get friend block.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        // Returns the matching FriendBlock? entity result or default if not found.
        public async Task<FriendBlock?> GetFriendBlock(int blockerId, int blockedId)
        {
            return await _context.FriendBlocks
                .FirstOrDefaultAsync(fb => fb.BlockerId == blockerId && fb.BlockedId == blockedId);  // Fetch single matching record or null if not found
        }

        // Persists state modifications to the database for add friend block.
        // Query details: commits entity state changes via EF Core SaveChangesAsync.
        public async Task AddFriendBlock(FriendBlock block)
        {
            _context.FriendBlocks.Add(block);
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
        }

        // Persists state modifications to the database for remove friend block.
        public async Task RemoveFriendBlock(FriendBlock block)
        {
            _context.FriendBlocks.Remove(block);  // Mark entity for deletion in the next SaveChanges call
            await _context.SaveChangesAsync();  // Flush all pending EF Core entity changes to the database
        }
    }
}
