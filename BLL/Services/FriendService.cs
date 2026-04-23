using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class FriendService : IFriendService
    {
        private readonly IFriendRepository _friendRepository;
        private readonly IPlayerProfileRepository _profileRepository;

        public FriendService(
            IFriendRepository friendRepository,
            IPlayerProfileRepository profileRepository)
        {
            _friendRepository = friendRepository;
            _profileRepository = profileRepository;
        }

        public async Task<FriendListResponseDto> GetFriendsAsync(Guid accountId)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new FriendListResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var friendships = await _friendRepository.GetFriendsAsync(profile.Id);

            var dtos = friendships.Select(f =>
            {
                var friend = f.RequesterId == profile.Id ? f.Addressee : f.Requester;
                return new FriendResponseDto
                {
                    FriendId = f.Id,
                    PlayerProfileId = friend!.Id,
                    PlayerDisplayName = friend.DisplayName,
                    PlayerAvatarUrl = friend.AvatarUrl,
                    PlayerLevel = friend.Level,
                    PlayerClass = friend.Class.ToString(),
                    Status = f.Status.ToString(),
                    CreatedAt = f.CreatedAt,
                    RespondedAt = f.RespondedAt
                };
            }).ToList();

            return new FriendListResponseDto
            {
                Success = true,
                Message = "Friends retrieved successfully.",
                Friends = dtos,
                TotalCount = dtos.Count
            };
        }

        public async Task<FriendListResponseDto> GetPendingRequestsAsync(Guid accountId)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new FriendListResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var requests = await _friendRepository.GetPendingRequestsAsync(profile.Id);

            var dtos = requests.Select(f => new FriendRequestResponseDto
            {
                RequestId = f.Id,
                RequesterId = f.RequesterId,
                RequesterDisplayName = f.Requester!.DisplayName,
                RequesterAvatarUrl = f.Requester.AvatarUrl,
                RequesterLevel = f.Requester.Level,
                RequesterClass = f.Requester.Class.ToString(),
                CreatedAt = f.CreatedAt
            }).ToList();

            return new FriendListResponseDto
            {
                Success = true,
                Message = "Pending requests retrieved successfully.",
                PendingRequests = dtos,
                TotalCount = dtos.Count
            };
        }

        public async Task<FriendListResponseDto> GetSentRequestsAsync(Guid accountId)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new FriendListResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var requests = await _friendRepository.GetSentRequestsAsync(profile.Id);

            var dtos = requests.Select(f => new FriendRequestResponseDto
            {
                RequestId = f.Id,
                RequesterId = f.AddresseeId,
                RequesterDisplayName = f.Addressee!.DisplayName,
                RequesterAvatarUrl = f.Addressee.AvatarUrl,
                RequesterLevel = f.Addressee.Level,
                RequesterClass = f.Addressee.Class.ToString(),
                CreatedAt = f.CreatedAt
            }).ToList();

            return new FriendListResponseDto
            {
                Success = true,
                Message = "Sent requests retrieved successfully.",
                PendingRequests = dtos,
                TotalCount = dtos.Count
            };
        }

        public async Task<FriendApiResponseDto> SendFriendRequestAsync(Guid accountId, SendFriendRequestDto request)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new FriendApiResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var addressee = await _profileRepository.GetByIdAsync(request.AddresseeId);
            if (addressee == null)
            {
                return new FriendApiResponseDto
                {
                    Success = false,
                    Message = "Target player not found."
                };
            }

            if (profile.Id == addressee.Id)
            {
                return new FriendApiResponseDto
                {
                    Success = false,
                    Message = "You cannot send a friend request to yourself."
                };
            }

            if (await _friendRepository.AreFriendsAsync(profile.Id, addressee.Id))
            {
                return new FriendApiResponseDto
                {
                    Success = false,
                    Message = "You are already friends with this player."
                };
            }

            if (await _friendRepository.HasPendingRequestAsync(profile.Id, addressee.Id))
            {
                return new FriendApiResponseDto
                {
                    Success = false,
                    Message = "A friend request already exists between you and this player."
                };
            }

            var friend = new Friend
            {
                Id = Guid.NewGuid(),
                RequesterId = profile.Id,
                AddresseeId = addressee.Id,
                Status = Friend.FriendStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _friendRepository.CreateAsync(friend);

            return new FriendApiResponseDto
            {
                Success = true,
                Message = $"Friend request sent to {addressee.DisplayName}!"
            };
        }

        public async Task<FriendApiResponseDto> RespondToRequestAsync(Guid accountId, RespondFriendRequestDto request)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new FriendApiResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var friend = await _friendRepository.GetByIdWithDetailsAsync(request.FriendId);
            if (friend == null || friend.AddresseeId != profile.Id)
            {
                return new FriendApiResponseDto
                {
                    Success = false,
                    Message = "Friend request not found."
                };
            }

            if (friend.Status != Friend.FriendStatus.Pending)
            {
                return new FriendApiResponseDto
                {
                    Success = false,
                    Message = "This friend request has already been responded to."
                };
            }

            if (request.Accept)
            {
                friend.Status = Friend.FriendStatus.Accepted;
                friend.RespondedAt = DateTime.UtcNow;
                await _friendRepository.UpdateAsync(friend);

                return new FriendApiResponseDto
                {
                    Success = true,
                    Message = $"You are now friends with {friend.Requester?.DisplayName}!"
                };
            }
            else
            {
                friend.Status = Friend.FriendStatus.Rejected;
                friend.RespondedAt = DateTime.UtcNow;
                await _friendRepository.UpdateAsync(friend);

                return new FriendApiResponseDto
                {
                    Success = true,
                    Message = "Friend request declined."
                };
            }
        }

        public async Task<FriendApiResponseDto> RemoveFriendAsync(Guid accountId, Guid friendId)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new FriendApiResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var friend = await _friendRepository.GetByIdAsync(friendId);
            if (friend == null ||
                (friend.RequesterId != profile.Id && friend.AddresseeId != profile.Id))
            {
                return new FriendApiResponseDto
                {
                    Success = false,
                    Message = "Friendship not found."
                };
            }

            await _friendRepository.DeleteAsync(friend);

            return new FriendApiResponseDto
            {
                Success = true,
                Message = "Friend removed successfully."
            };
        }

        public async Task<FriendApiResponseDto> BlockPlayerAsync(Guid accountId, Guid playerId)
        {
            var profile = await _profileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return new FriendApiResponseDto
                {
                    Success = false,
                    Message = "Player profile not found."
                };
            }

            var target = await _profileRepository.GetByIdAsync(playerId);
            if (target == null)
            {
                return new FriendApiResponseDto
                {
                    Success = false,
                    Message = "Target player not found."
                };
            }

            var existing = await _friendRepository.GetExistingFriendshipAsync(profile.Id, target.Id);
            if (existing != null)
            {
                existing.Status = Friend.FriendStatus.Blocked;
                await _friendRepository.UpdateAsync(existing);
            }
            else
            {
                var block = new Friend
                {
                    Id = Guid.NewGuid(),
                    RequesterId = profile.Id,
                    AddresseeId = target.Id,
                    Status = Friend.FriendStatus.Blocked,
                    CreatedAt = DateTime.UtcNow
                };
                await _friendRepository.CreateAsync(block);
            }

            return new FriendApiResponseDto
            {
                Success = true,
                Message = $"Player {target.DisplayName} has been blocked."
            };
        }
    }
}
