using DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IFriendRepository
    {
        Task<Friend?> GetByIdAsync(Guid friendId);
        Task<Friend?> GetByIdWithDetailsAsync(Guid friendId);
        Task<List<Friend>> GetFriendsAsync(Guid playerProfileId);
        Task<List<Friend>> GetPendingRequestsAsync(Guid playerProfileId);
        Task<List<Friend>> GetSentRequestsAsync(Guid playerProfileId);
        Task<Friend?> GetExistingFriendshipAsync(Guid requesterId, Guid addresseeId);
        Task<bool> AreFriendsAsync(Guid player1Id, Guid player2Id);
        Task<bool> HasPendingRequestAsync(Guid requesterId, Guid addresseeId);
        Task<Friend> CreateAsync(Friend friend);
        Task<Friend> UpdateAsync(Friend friend);
        Task DeleteAsync(Friend friend);
    }
}
