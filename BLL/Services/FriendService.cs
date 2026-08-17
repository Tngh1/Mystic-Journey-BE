using BLL.DTOs;
using BLL.Services.Interfaces;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    // Executes core business logic for i friend service.
    public class FriendService : IFriendService
    {
        private readonly IFriendRepository _friendRepository;
        private readonly IPlayerProfileRepository _playerProfileRepository;
        private readonly IChatMessageRepository _chatMessageRepository;
        private readonly IDistributedCache _cache;
        private readonly IPlayerHeartbeatService _heartbeatService;

        // Initialize this instance from friend repository, player profile repository, chat message repository, and cache and store friend repository, player profile repository, chat message repository, cache, and heartbeat service for later operations.
        public FriendService(
            IFriendRepository friendRepository,
            IPlayerProfileRepository playerProfileRepository,
            IChatMessageRepository chatMessageRepository,
            IDistributedCache cache,
            IPlayerHeartbeatService heartbeatService)
        {
            _friendRepository = friendRepository;
            _playerProfileRepository = playerProfileRepository;
            _chatMessageRepository = chatMessageRepository;
            _cache = cache;
            _heartbeatService = heartbeatService;
        }

        // Executes core business logic for get friend list.
        // Logic details: delegates data queries and updates to repository layer.
        // Returns the computed List<FriendDto result asynchronously.
        public async Task<List<FriendDto>> GetFriendList(int playerId)
        {
            var friends = await _friendRepository.GetFriendListRaw(playerId);
            return friends.Select(f =>
            {
                var friendProfile = f.RequesterId == playerId ? f.Addressee : f.Requester;
                return new FriendDto
                {
                    FriendshipId = f.FriendId,
                    FriendProfileId = friendProfile!.PlayerProfileId,
                    FriendName = friendProfile.DisplayName,
                    FriendLevel = friendProfile.Level,
                    FriendAvatarUrl = friendProfile.AvatarUrl,
                    Class = friendProfile.Class,
                    CurrentMap = "World Map",
                    IsInDungeon = false,
                    CanInvite = true,
                    LastOnline = friendProfile.UpdatedAt ?? friendProfile.CreatedAt,
                    IsOnline = IsPlayerOnline(friendProfile),
                    Status = f.Status
                };
            }).ToList();
        }

        // Executes core business logic for get friend requests.
        // Logic details: delegates data queries and updates to repository layer.
        // Returns the computed List<PendingFriendRequestDto result asynchronously.
        public async Task<List<PendingFriendRequestDto>> GetFriendRequests(int playerId)
        {
            var requests = await _friendRepository.GetFriendRequests(playerId);
            return requests.Select(f => new PendingFriendRequestDto
            {
                FriendshipId = f.FriendId,
                RequesterId = f.Requester!.PlayerProfileId,
                RequesterName = f.Requester.DisplayName,
                RequesterLevel = f.Requester.Level,
                RequesterAvatarUrl = f.Requester.AvatarUrl,
                Class = f.Requester.Class,
                CreatedAt = f.CreatedAt
            }).ToList();
        }

        // Executes core business logic for get friend blocks.
        // Logic details: delegates data queries and updates to repository layer.
        // Returns the computed List<FriendProfileDto result asynchronously.
        public async Task<List<FriendProfileDto>> GetFriendBlocks(int playerId)
        {
            var blocks = await _friendRepository.GetFriendBlocks(playerId);
            return blocks.Select(b => new FriendProfileDto
            {
                ProfileId = b.Blocked!.PlayerProfileId,
                CharacterName = b.Blocked.DisplayName,
                Class = b.Blocked.Class,
                Level = b.Blocked.Level,
                AvatarUrl = b.Blocked.AvatarUrl
            }).ToList();
        }

        // Executes core business logic for get friend profile.
        // Logic details: delegates data queries and updates to repository layer.
        // Returns the computed FriendProfileDto? result asynchronously.
        public async Task<FriendProfileDto?> GetFriendProfile(int profileId)
        {
            var profile = await _playerProfileRepository.GetPlayerProfileById(profileId);
            if (profile == null) return null;  // Entity not found — short-circuit with appropriate error result

            return new FriendProfileDto
            {
                ProfileId = profile.PlayerProfileId,
                CharacterName = profile.DisplayName,
                Class = profile.Class,
                Level = profile.Level,
                Power = profile.Level * 100,
                Guild = "No Guild",
                AvatarUrl = profile.AvatarUrl,
                Title = "Novice",
                LastOnline = profile.UpdatedAt ?? profile.CreatedAt,
                IsOnline = IsPlayerOnline(profile),
                HasChangedName = profile.HasChangedName
            };
        }

        // Executes core business logic for search players.
        // Logic details: delegates data queries and updates to repository layer.
        // Returns the computed List<FriendSearchDto result asynchronously.
        public async Task<List<FriendSearchDto>> SearchPlayers(int playerId, string keyword)
        {
            var profiles = (await _playerProfileRepository.Search(keyword)).Take(20).ToList();  // Apply pagination limit — cap result set size
            var results = new List<FriendSearchDto>();

            var otherIds = profiles
                .Where(p => p.PlayerProfileId != playerId)  // Filter records matching the predicate
                .Select(p => p.PlayerProfileId)
                .ToList();

            var blockedIds = otherIds.Count == 0
                ? new HashSet<int>()
                : (await _friendRepository.GetFriendBlocksWith(playerId, otherIds))
                    .Select(fb => fb.BlockedId)
                    .ToHashSet();

            var friendships = otherIds.Count == 0
                ? new Dictionary<int, Friend>()
                : (await _friendRepository.GetFriendshipsWith(playerId, otherIds))
                    .GroupBy(f => f.RequesterId == playerId ? f.AddresseeId : f.RequesterId)  // Aggregate records by grouping key
                    .ToDictionary(g => g.Key, g => g.First());

            foreach (var p in profiles)
            {
                // Supported friendship states: Pending or Accepted; Pending is unanswered and Accepted is an active friendship.
                var status = FriendRelationshipStatus.None;

                if (p.PlayerProfileId == playerId)
                {
                    status = FriendRelationshipStatus.Self;
                }
                else if (blockedIds.Contains(p.PlayerProfileId))
                {
                    status = FriendRelationshipStatus.Blocked;
                }
                else if (friendships.TryGetValue(p.PlayerProfileId, out var friendship))
                {
                    if (friendship.Status == "Accepted")
                    {
                        status = FriendRelationshipStatus.Friend;
                    }
                    else if (friendship.Status == "Pending")
                    {
                        status = friendship.RequesterId == playerId
                            ? FriendRelationshipStatus.RequestSent
                            : FriendRelationshipStatus.RequestReceived;
                    }
                }

                results.Add(new FriendSearchDto
                {
                    ProfileId = p.PlayerProfileId,
                    CharacterName = p.DisplayName,
                    Level = p.Level,
                    Class = p.Class,
                    Avatar = p.AvatarUrl,
                    Power = p.Level * 100,
                    GuildName = "No Guild",
                    IsOnline = IsPlayerOnline(p),
                    RelationshipStatus = status
                });
            }

            return results;
        }

        // Executes core business logic for send friend request.
        // Logic details: delegates data queries and updates to repository layer.
        // Completes asynchronously upon successful execution.
        public async Task SendFriendRequest(int requesterId, int targetProfileId)
        {
            if (requesterId == targetProfileId) throw new Exception("Cannot send friend request to yourself");

            var targetProfile = await _playerProfileRepository.GetPlayerProfileById(targetProfileId);
            if (targetProfile == null) throw new Exception("Player not found");  // Entity not found — short-circuit with appropriate error result

            var block = await _friendRepository.GetFriendBlock(targetProfileId, requesterId);
            if (block != null) throw new Exception("Cannot send friend request");  // Entity exists — proceed with conditional branch

            var reverseBlock = await _friendRepository.GetFriendBlock(requesterId, targetProfileId);
            if (reverseBlock != null) throw new Exception("You have blocked this player");  // Entity exists — proceed with conditional branch

            if (await _friendRepository.CountFriends(requesterId) >= 100)
                throw new Exception("Friend list is full (Limit: 100)");

            if (await _friendRepository.CountFriends(targetProfileId) >= 100)
                throw new Exception("Target's friend list is full");

            var existing = await _friendRepository.GetFriendship(requesterId, targetProfileId);
            if (existing != null)  // Entity exists — proceed with conditional branch
            {
                if (existing.Status == "Accepted") throw new Exception("Already friends");
                if (existing.Status == "Pending")
                {
                    if (existing.AddresseeId == requesterId)
                    {
                        throw new Exception("You already have a pending friend request from this player. Please accept it.");
                    }
                    throw new Exception("Friend request already sent");
                }

                existing.Status = "Pending";
                existing.RequesterId = requesterId;
                existing.AddresseeId = targetProfileId;
                existing.CreatedAt = DateTime.UtcNow;
                await _friendRepository.UpdateFriend(existing);
                return;
            }

            var friend = new Friend
            {
                RequesterId = requesterId,
                AddresseeId = targetProfileId,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };
            await _friendRepository.AddFriend(friend);
        }

        // Executes core business logic for accept friend request.
        // Logic details: delegates data queries and updates to repository layer.
        // Completes asynchronously upon successful execution.
        public async Task AcceptFriendRequest(int playerId, int requesterId)
        {
            var friendship = await _friendRepository.GetFriendship(playerId, requesterId);
            if (friendship == null || friendship.Status != "Pending" || friendship.AddresseeId != playerId)
            {
                throw new Exception("Friend request not found or cannot be accepted");
            }

            if (await _friendRepository.CountFriends(playerId) >= 100)
                throw new Exception("Friend list is full (Limit: 100)");

            if (await _friendRepository.CountFriends(requesterId) >= 100)
                throw new Exception("Requester's friend list is full");

            friendship.Status = "Accepted";
            friendship.RespondedAt = DateTime.UtcNow;
            await _friendRepository.UpdateFriend(friendship);
        }

        // Executes core business logic for decline friend request.
        // Logic details: delegates data queries and updates to repository layer.
        // Completes asynchronously upon successful execution.
        public async Task DeclineFriendRequest(int playerId, int requesterId)
        {
            var friendship = await _friendRepository.GetFriendship(playerId, requesterId);
            if (friendship == null || friendship.Status != "Pending" || friendship.AddresseeId != playerId)
            {
                throw new Exception("Friend request not found or cannot be declined");
            }

            friendship.Status = "Rejected";
            friendship.RespondedAt = DateTime.UtcNow;
            await _friendRepository.UpdateFriend(friendship);
        }

        // Executes core business logic for remove friend.
        // Logic details: delegates data queries and updates to repository layer.
        // Completes asynchronously upon successful execution.
        public async Task RemoveFriend(int playerId, int targetId)
        {
            var friendship = await _friendRepository.GetFriendship(playerId, targetId);

            await DeleteFriendConversation(playerId, targetId);

            if (friendship != null)  // Entity exists — proceed with conditional branch
            {
                await _friendRepository.RemoveFriend(friendship);
            }
        }

        // Executes core business logic for block player.
        // Logic details: delegates data queries and updates to repository layer.
        // Completes asynchronously upon successful execution.
        public async Task BlockPlayer(int playerId, int targetProfileId)
        {
            if (playerId == targetProfileId) throw new Exception("Cannot block yourself");

            var targetProfile = await _playerProfileRepository.GetPlayerProfileById(targetProfileId);
            if (targetProfile == null) throw new Exception("Player not found");  // Entity not found — short-circuit with appropriate error result

            var friendship = await _friendRepository.GetFriendship(playerId, targetProfileId);
            await DeleteFriendConversation(playerId, targetProfileId);
            if (friendship != null)  // Entity exists — proceed with conditional branch
            {
                await _friendRepository.RemoveFriend(friendship);
            }

            var existingBlock = await _friendRepository.GetFriendBlock(playerId, targetProfileId);
            if (existingBlock == null)  // Entity not found — short-circuit with appropriate error result
            {
                var block = new FriendBlock
                {
                    BlockerId = playerId,
                    BlockedId = targetProfileId,
                    CreatedAt = DateTime.UtcNow
                };
                await _friendRepository.AddFriendBlock(block);
            }
        }

        // Executes core business logic for unblock player.
        // Logic details: delegates data queries and updates to repository layer.
        // Completes asynchronously upon successful execution.
        public async Task UnblockPlayer(int playerId, int targetProfileId)
        {
            var existingBlock = await _friendRepository.GetFriendBlock(playerId, targetProfileId);
            if (existingBlock != null)  // Entity exists — proceed with conditional branch
            {
                await _friendRepository.RemoveFriendBlock(existingBlock);
            }
        }

        // Executes core business logic for is player online.
        // Logic details: validates numeric boundary constraints; delegates data queries and updates to repository layer.
        // Returns a boolean indicating operation success.
        private bool IsPlayerOnline(PlayerProfile? profile)
        {
            if (profile == null)  // Entity not found — short-circuit with appropriate error result
                return false;

            return _heartbeatService.IsOnline(profile.LastSeen);
        }

        // Executes core business logic for delete friend conversation.
        // Logic details: validates numeric boundary constraints; delegates data queries and updates to repository layer.
        // Completes asynchronously upon successful execution.
        private async Task DeleteFriendConversation(int firstPlayerProfileId, int secondPlayerProfileId)
        {
            if (firstPlayerProfileId <= 0 || secondPlayerProfileId <= 0 || firstPlayerProfileId == secondPlayerProfileId)
                return;

            await _chatMessageRepository.DeleteConversation(firstPlayerProfileId, secondPlayerProfileId);
            await RemoveConversationCache(firstPlayerProfileId, secondPlayerProfileId);
        }

        // Executes core business logic for remove conversation cache.
        // Completes asynchronously upon successful execution.
        private async Task RemoveConversationCache(int firstPlayerProfileId, int secondPlayerProfileId)
        {
            try
            {
                await _cache.RemoveAsync(GetConversationCacheKey(firstPlayerProfileId, secondPlayerProfileId));  // Invalidate stale cache entry to force fresh recalculation
            }
            catch
            {
            }
        }

        // Executes core business logic for get conversation cache key.
        private static string GetConversationCacheKey(int firstPlayerProfileId, int secondPlayerProfileId)
        {
            var min = Math.Min(firstPlayerProfileId, secondPlayerProfileId);
            var max = Math.Max(firstPlayerProfileId, secondPlayerProfileId);
            return $"chat:conversation:{min}:{max}:latest";
        }
    }
}
