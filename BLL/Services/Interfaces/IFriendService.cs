using BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IFriendService
    {
        Task<FriendListResponseDto> GetFriendsAsync(Guid accountId);
        Task<FriendListResponseDto> GetPendingRequestsAsync(Guid accountId);
        Task<FriendListResponseDto> GetSentRequestsAsync(Guid accountId);
        Task<FriendApiResponseDto> SendFriendRequestAsync(Guid accountId, SendFriendRequestDto request);
        Task<FriendApiResponseDto> RespondToRequestAsync(Guid accountId, RespondFriendRequestDto request);
        Task<FriendApiResponseDto> RemoveFriendAsync(Guid accountId, Guid friendId);
        Task<FriendApiResponseDto> BlockPlayerAsync(Guid accountId, Guid playerId);
    }
}
