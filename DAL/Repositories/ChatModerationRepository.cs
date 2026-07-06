using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class ChatModerationRepository : IChatModerationRepository
    {
        private readonly MysticJourneyDbContext _context;

        public ChatModerationRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<ChatModerationPenalty?> GetActivePenalty(int playerProfileId, DateTime now)
        {
            return await _context.ChatModerationPenalties
                .Where(x => x.PlayerProfileId == playerProfileId && x.LockedUntil > now)
                .OrderByDescending(x => x.LockedUntil)
                .ThenByDescending(x => x.ChatModerationPenaltyId)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<int> CountPenalties(int playerProfileId)
        {
            return await _context.ChatModerationPenalties
                .CountAsync(x => x.PlayerProfileId == playerProfileId);
        }

        public async Task<ChatModerationPenalty?> GetPenaltyByChatMessageId(int chatMessageId)
        {
            return await _context.ChatModerationPenalties
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ChatMessageId == chatMessageId);
        }

        public async Task<ChatModerationPenalty?> GetPenaltyByWorldMessageId(int worldChatMessageId)
        {
            return await _context.ChatModerationPenalties
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.WorldChatMessageId == worldChatMessageId);
        }

        public async Task<ChatModerationPenalty> Create(ChatModerationPenalty penalty)
        {
            await _context.ChatModerationPenalties.AddAsync(penalty);
            await _context.SaveChangesAsync();
            return penalty;
        }
    }
}