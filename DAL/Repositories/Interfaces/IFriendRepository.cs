using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Initializes a new default instance of the IFriendRepository class.
    public interface IFriendRepository
    {
        Task<List<Friend>> GetFriendListRaw(int playerProfileId);
        Task<List<Friend>> GetFriendRequests(int playerProfileId);
        Task<Friend?> GetFriendship(int id1, int id2);

        Task<int> CountFriends(int playerProfileId);

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
