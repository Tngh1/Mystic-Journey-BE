using DAL.Models;
using System;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IChatModerationRepository
    {
        Task<ChatModerationPenalty?> GetActivePenalty(int playerProfileId, DateTime now);
        Task<int> CountPenalties(int playerProfileId);
        Task<ChatModerationPenalty?> GetPenaltyByChatMessageId(int chatMessageId);
        Task<ChatModerationPenalty?> GetPenaltyByWorldMessageId(int worldChatMessageId);
        Task<ChatModerationPenalty> Create(ChatModerationPenalty penalty);
    }
}