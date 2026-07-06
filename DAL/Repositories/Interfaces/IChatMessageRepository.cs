using DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IChatMessageRepository
    {
        Task<(int TotalCount, List<WorldChatMessage> Items)> GetWorldMessagesPaged(
            int page,
            int pageSize);

        Task<DateTime?> GetLatestWorldSentAtBySenderId(int senderId);
        Task<WorldChatMessage> CreateWorldMessage(WorldChatMessage message);
        Task<WorldChatMessage?> GetWorldMessageById(int worldChatMessageId);
        Task<WorldChatMessage> UpdateWorldMessage(WorldChatMessage message);

        Task<(int TotalCount, List<ChatMessage> Items)> GetConversationPaged(
            int playerProfileId,
            int otherPlayerProfileId,
            int page,
            int pageSize);

        Task<DateTime?> GetLatestSentAtBySenderId(int senderId);
        Task<ChatMessage> Create(ChatMessage message);
        Task<ChatMessage?> GetMessageById(int chatMessageId);
        Task<ChatMessage> Update(ChatMessage message);
    }
}
