using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    /// <summary>
    /// Triển khai các thao tác truy cập dữ liệu cho mối quan hệ kết bạn sử dụng Entity Framework.
    /// </summary>
    public class FriendRepository : IFriendRepository
    {
        private readonly MysticJourneyDbContext _context;

        public FriendRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách bạn bè của người chơi.
        /// Trả về profile của người bạn (không phải người gửi/yêu cầu).
        /// Chỉ lấy các mối quan hệ có trạng thái "Accepted".
        /// </summary>
        public async Task<List<PlayerProfile>> GetFriends(int playerProfileId)
        {
            return await _context.Friends
                .Where(f => (f.RequesterId == playerProfileId || f.AddresseeId == playerProfileId) && f.Status == "Accepted")
                .Include(f => f.Requester)
                .Include(f => f.Addressee)
                .Select(f => f.RequesterId == playerProfileId ? f.Addressee! : f.Requester!)
                .Where(p => p != null)
                .ToListAsync();
        }
        public async Task<List<Friend>> GetFriendListRaw(int playerProfileId)
        {
            return await _context.Friends
                .Include(f => f.Requester)
                .Include(f => f.Addressee)
                .Where(f => (f.RequesterId == playerProfileId || f.AddresseeId == playerProfileId) && f.Status == "Accepted")
                .ToListAsync();
        }

        public async Task<List<Friend>> GetFriendRequests(int playerProfileId)
        {
            return await _context.Friends
                .Include(f => f.Requester)
                .Include(f => f.Addressee)
                .Where(f => f.AddresseeId == playerProfileId && f.Status == "Pending")
                .ToListAsync();
        }

        public async Task<Friend?> GetFriendship(int id1, int id2)
        {
            return await _context.Friends
                .FirstOrDefaultAsync(f => 
                    (f.RequesterId == id1 && f.AddresseeId == id2) || 
                    (f.RequesterId == id2 && f.AddresseeId == id1));
        }

        public async Task<Friend> AddFriend(Friend friend)
        {
            _context.Friends.Add(friend);
            await _context.SaveChangesAsync();
            return friend;
        }

        public async Task UpdateFriend(Friend friend)
        {
            _context.Friends.Update(friend);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFriend(Friend friend)
        {
            _context.Friends.Remove(friend);
            await _context.SaveChangesAsync();
        }

        public async Task<List<FriendBlock>> GetFriendBlocks(int playerProfileId)
        {
            return await _context.FriendBlocks
                .Include(fb => fb.Blocked)
                .Where(fb => fb.BlockerId == playerProfileId)
                .ToListAsync();
        }

        public async Task<FriendBlock?> GetFriendBlock(int blockerId, int blockedId)
        {
            return await _context.FriendBlocks
                .FirstOrDefaultAsync(fb => fb.BlockerId == blockerId && fb.BlockedId == blockedId);
        }

        public async Task AddFriendBlock(FriendBlock block)
        {
            _context.FriendBlocks.Add(block);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFriendBlock(FriendBlock block)
        {
            _context.FriendBlocks.Remove(block);
            await _context.SaveChangesAsync();
        }
    }
}
