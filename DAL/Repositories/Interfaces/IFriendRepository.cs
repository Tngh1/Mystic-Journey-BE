using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IFriendRepository
    {
        Task<List<Friend>> GetFriendListRaw(int playerProfileId);
        Task<List<Friend>> GetFriendRequests(int playerProfileId);
        Task<Friend?> GetFriendship(int id1, int id2);

        // Đếm số bạn đã kết bạn — dùng cho giới hạn 100, khỏi nạp cả graph profile+account.
        Task<int> CountFriends(int playerProfileId);

        // Lấy quan hệ / block giữa một người chơi và nhiều người khác trong 1 truy vấn
        // (tìm kiếm bạn bè trả về tới 20 kết quả, hỏi từng người là 40 round-trip).
        Task<List<Friend>> GetFriendshipsWith(int playerProfileId, List<int> otherIds);
        Task<List<FriendBlock>> GetFriendBlocksWith(int blockerId, List<int> blockedIds);
        Task<Friend> AddFriend(Friend friend);
        Task UpdateFriend(Friend friend);
        Task RemoveFriend(Friend friend);
        Task<List<FriendBlock>> GetFriendBlocks(int playerProfileId);
        Task<FriendBlock?> GetFriendBlock(int blockerId, int blockedId);
        Task AddFriendBlock(FriendBlock block);
        Task RemoveFriendBlock(FriendBlock block);
        Task<List<PlayerProfile>> GetFriends(int playerProfileId);
    }
}
