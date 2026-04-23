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
    public class FriendRepository : IFriendRepository
    {
        private readonly MysticJourneyDbContext _context;

        public FriendRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<Friend?> GetByIdAsync(Guid friendId)
        {
            return await _context.Friends
                .FirstOrDefaultAsync(f => f.Id == friendId);
        }

        public async Task<Friend?> GetByIdWithDetailsAsync(Guid friendId)
        {
            return await _context.Friends
                .Include(f => f.Requester)
                .Include(f => f.Addressee)
                .FirstOrDefaultAsync(f => f.Id == friendId);
        }

        public async Task<List<Friend>> GetFriendsAsync(Guid playerProfileId)
        {
            return await _context.Friends
                .Include(f => f.Requester)
                .Include(f => f.Addressee)
                .Where(f => (f.RequesterId == playerProfileId || f.AddresseeId == playerProfileId) &&
                            f.Status == Friend.FriendStatus.Accepted)
                .OrderByDescending(f => f.RespondedAt)
                .ToListAsync();
        }

        public async Task<List<Friend>> GetPendingRequestsAsync(Guid playerProfileId)
        {
            return await _context.Friends
                .Include(f => f.Requester)
                .Where(f => f.AddresseeId == playerProfileId && f.Status == Friend.FriendStatus.Pending)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Friend>> GetSentRequestsAsync(Guid playerProfileId)
        {
            return await _context.Friends
                .Include(f => f.Addressee)
                .Where(f => f.RequesterId == playerProfileId && f.Status == Friend.FriendStatus.Pending)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<Friend?> GetExistingFriendshipAsync(Guid requesterId, Guid addresseeId)
        {
            return await _context.Friends
                .FirstOrDefaultAsync(f =>
                    (f.RequesterId == requesterId && f.AddresseeId == addresseeId) ||
                    (f.RequesterId == addresseeId && f.AddresseeId == requesterId));
        }

        public async Task<bool> AreFriendsAsync(Guid player1Id, Guid player2Id)
        {
            return await _context.Friends
                .AnyAsync(f =>
                    ((f.RequesterId == player1Id && f.AddresseeId == player2Id) ||
                     (f.RequesterId == player2Id && f.AddresseeId == player1Id)) &&
                    f.Status == Friend.FriendStatus.Accepted);
        }

        public async Task<bool> HasPendingRequestAsync(Guid requesterId, Guid addresseeId)
        {
            return await _context.Friends
                .AnyAsync(f =>
                    ((f.RequesterId == requesterId && f.AddresseeId == addresseeId) ||
                     (f.RequesterId == addresseeId && f.AddresseeId == requesterId)) &&
                    f.Status == Friend.FriendStatus.Pending);
        }

        public async Task<Friend> CreateAsync(Friend friend)
        {
            await _context.Friends.AddAsync(friend);
            await _context.SaveChangesAsync();
            return friend;
        }

        public async Task<Friend> UpdateAsync(Friend friend)
        {
            _context.Friends.Update(friend);
            await _context.SaveChangesAsync();
            return friend;
        }

        public async Task DeleteAsync(Friend friend)
        {
            _context.Friends.Remove(friend);
            await _context.SaveChangesAsync();
        }
    }
}
