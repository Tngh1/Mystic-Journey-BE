using BLL.DTOs;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class FriendService : Interfaces.IFriendService
    {
        private readonly IFriendRepository _friendRepository;
        private readonly IPlayerProfileRepository _playerProfileRepository;
        private readonly IChatMessageRepository _chatMessageRepository;
        private readonly IDistributedCache _cache;
        private readonly Interfaces.IPlayerPresenceService _presenceService;

        public FriendService(
            IFriendRepository friendRepository,
            IPlayerProfileRepository playerProfileRepository,
            IChatMessageRepository chatMessageRepository,
            IDistributedCache cache,
            Interfaces.IPlayerPresenceService presenceService)
        {
            _friendRepository = friendRepository;
            _playerProfileRepository = playerProfileRepository;
            _chatMessageRepository = chatMessageRepository;
            _cache = cache;
            _presenceService = presenceService;
        }

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
                    CurrentMap = "World Map", // Future integration
                    IsInDungeon = false, // Future integration
                    CanInvite = true, // Future integration
                    LastOnline = friendProfile.UpdatedAt ?? friendProfile.CreatedAt,
                    IsOnline = _presenceService.IsOnline(friendProfile.PlayerProfileId),
                    Status = f.Status
                };
            }).ToList();
        }

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

        public async Task<FriendProfileDto?> GetFriendProfile(int profileId)
        {
            var profile = await _playerProfileRepository.GetPlayerProfileById(profileId);
            if (profile == null) return null;

            return new FriendProfileDto
            {
                ProfileId = profile.PlayerProfileId,
                CharacterName = profile.DisplayName,
                Class = profile.Class,
                Level = profile.Level,
                Power = profile.Level * 100, // Mocked Power
                Guild = "No Guild", // Mocked
                AvatarUrl = profile.AvatarUrl,
                Title = "Novice", // Mocked
                LastOnline = profile.UpdatedAt ?? profile.CreatedAt,
                IsOnline = _presenceService.IsOnline(profileId)
            };
        }

        public async Task<List<FriendSearchDto>> SearchPlayers(int playerId, string keyword)
        {
            var profiles = await _playerProfileRepository.Search(keyword);
            var results = new List<FriendSearchDto>();

            foreach (var p in profiles.Take(20))
            {
                var status = FriendRelationshipStatus.None;

                if (p.PlayerProfileId == playerId)
                {
                    status = FriendRelationshipStatus.Self;
                }
                else
                {
                    // Check block
                    var block = await _friendRepository.GetFriendBlock(playerId, p.PlayerProfileId);
                    if (block != null)
                    {
                        status = FriendRelationshipStatus.Blocked;
                    }
                    else
                    {
                        var friendship = await _friendRepository.GetFriendship(playerId, p.PlayerProfileId);
                        if (friendship != null)
                        {
                            if (friendship.Status == "Accepted")
                            {
                                status = FriendRelationshipStatus.Friend;
                            }
                            else if (friendship.Status == "Pending")
                            {
                                if (friendship.RequesterId == playerId)
                                    status = FriendRelationshipStatus.RequestSent;
                                else
                                    status = FriendRelationshipStatus.RequestReceived;
                            }
                        }
                    }
                }

                results.Add(new FriendSearchDto
                {
                    ProfileId = p.PlayerProfileId,
                    CharacterName = p.DisplayName,
                    Level = p.Level,
                    Class = p.Class,
                    Avatar = p.AvatarUrl,
                    Power = p.Level * 100, // Mocked
                    GuildName = "No Guild", // Mocked
                    IsOnline = _presenceService.IsOnline(p.PlayerProfileId),
                    RelationshipStatus = status
                });
            }

            return results;
        }

        public async Task SendFriendRequest(int requesterId, int targetProfileId)
        {
            if (requesterId == targetProfileId) throw new Exception("Cannot send friend request to yourself");

            var targetProfile = await _playerProfileRepository.GetPlayerProfileById(targetProfileId);
            if (targetProfile == null) throw new Exception("Player not found");

            // Check if blocked
            var block = await _friendRepository.GetFriendBlock(targetProfileId, requesterId);
            if (block != null) throw new Exception("Cannot send friend request"); // target blocked requester

            var reverseBlock = await _friendRepository.GetFriendBlock(requesterId, targetProfileId);
            if (reverseBlock != null) throw new Exception("You have blocked this player");

            // Check friends limit (requester)
            var currentFriends = await _friendRepository.GetFriendListRaw(requesterId);
            if (currentFriends.Count >= 100) throw new Exception("Friend list is full (Limit: 100)");

            // Check friends limit (target)
            var targetFriends = await _friendRepository.GetFriendListRaw(targetProfileId);
            if (targetFriends.Count >= 100) throw new Exception("Target's friend list is full");

            var existing = await _friendRepository.GetFriendship(requesterId, targetProfileId);
            if (existing != null)
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
                
                // If rejected, we allow re-sending
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

        public async Task AcceptFriendRequest(int playerId, int requesterId)
        {
            var friendship = await _friendRepository.GetFriendship(playerId, requesterId);
            if (friendship == null || friendship.Status != "Pending" || friendship.AddresseeId != playerId)
            {
                throw new Exception("Friend request not found or cannot be accepted");
            }

            // Check limits again
            var currentFriends = await _friendRepository.GetFriendListRaw(playerId);
            if (currentFriends.Count >= 100) throw new Exception("Friend list is full (Limit: 100)");

            var targetFriends = await _friendRepository.GetFriendListRaw(requesterId);
            if (targetFriends.Count >= 100) throw new Exception("Requester's friend list is full");

            friendship.Status = "Accepted";
            friendship.RespondedAt = DateTime.UtcNow;
            await _friendRepository.UpdateFriend(friendship);
        }

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

        public async Task RemoveFriend(int playerId, int targetId)
        {
            var friendship = await _friendRepository.GetFriendship(playerId, targetId);

            await DeleteFriendConversation(playerId, targetId);

            if (friendship != null)
            {
                await _friendRepository.RemoveFriend(friendship);
            }
        }

        public async Task BlockPlayer(int playerId, int targetProfileId)
        {
            if (playerId == targetProfileId) throw new Exception("Cannot block yourself");

            var targetProfile = await _playerProfileRepository.GetPlayerProfileById(targetProfileId);
            if (targetProfile == null) throw new Exception("Player not found");

            // If they are friends, unfriend them first
            var friendship = await _friendRepository.GetFriendship(playerId, targetProfileId);
            await DeleteFriendConversation(playerId, targetProfileId);
            if (friendship != null)
            {
                await _friendRepository.RemoveFriend(friendship);
            }

            var existingBlock = await _friendRepository.GetFriendBlock(playerId, targetProfileId);
            if (existingBlock == null)
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

        public async Task UnblockPlayer(int playerId, int targetProfileId)
        {
            var existingBlock = await _friendRepository.GetFriendBlock(playerId, targetProfileId);
            if (existingBlock != null)
            {
                await _friendRepository.RemoveFriendBlock(existingBlock);
            }
        }
        private async Task DeleteFriendConversation(int firstPlayerProfileId, int secondPlayerProfileId)
        {
            if (firstPlayerProfileId <= 0 || secondPlayerProfileId <= 0 || firstPlayerProfileId == secondPlayerProfileId)
                return;

            await _chatMessageRepository.DeleteConversation(firstPlayerProfileId, secondPlayerProfileId);
            await RemoveConversationCache(firstPlayerProfileId, secondPlayerProfileId);
        }

        private async Task RemoveConversationCache(int firstPlayerProfileId, int secondPlayerProfileId)
        {
            try
            {
                await _cache.RemoveAsync(GetConversationCacheKey(firstPlayerProfileId, secondPlayerProfileId));
            }
            catch
            {
                // Redis/cache cleanup failure must not block unfriending.
            }
        }

        private static string GetConversationCacheKey(int firstPlayerProfileId, int secondPlayerProfileId)
        {
            var min = Math.Min(firstPlayerProfileId, secondPlayerProfileId);
            var max = Math.Max(firstPlayerProfileId, secondPlayerProfileId);
            return $"chat:conversation:{min}:{max}:latest";
        }
    }
}
