using BLL.DTOs;
using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IFriendService
    {
        Task<List<FriendDto>> GetFriendList(int playerId);
        Task<List<PendingFriendRequestDto>> GetFriendRequests(int playerId);
        Task<List<FriendProfileDto>> GetFriendBlocks(int playerId);
        Task<FriendProfileDto?> GetFriendProfile(int profileId);
        Task<List<FriendSearchDto>> SearchPlayers(int playerId, string keyword);
        Task SendFriendRequest(int requesterId, int targetProfileId);
        Task AcceptFriendRequest(int playerId, int requesterId);
        Task DeclineFriendRequest(int playerId, int requesterId);
        Task RemoveFriend(int playerId, int targetId);
        Task BlockPlayer(int playerId, int targetProfileId);
        Task UnblockPlayer(int playerId, int targetProfileId);
    }
}
