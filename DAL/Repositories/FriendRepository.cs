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
    }
}
