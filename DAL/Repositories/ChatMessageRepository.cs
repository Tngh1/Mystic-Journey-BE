using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class ChatMessageRepository : IChatMessageRepository
    {
        private readonly MysticJourneyDbContext _context;

        public ChatMessageRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<(int TotalCount, List<WorldChatMessage> Items)> GetWorldMessagesPaged(
            int page,
            int pageSize)
        {
            page = Math.Max(1, page);
            pageSize = Math.Max(1, pageSize);

            var query = _context.WorldChatMessages
                .Include(m => m.Sender)
                .Where(m => !m.IsHidden)
                .AsNoTracking();

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(m => m.SentAt)
                .ThenByDescending(m => m.WorldChatMessageId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (totalCount, items);
        }

        public async Task<DateTime?> GetLatestWorldSentAtBySenderId(int senderId)
        {
            return await _context.WorldChatMessages
                .Where(m => m.SenderId == senderId)
                .OrderByDescending(m => m.SentAt)
                .Select(m => (DateTime?)m.SentAt)
                .FirstOrDefaultAsync();
        }

        public async Task<WorldChatMessage> CreateWorldMessage(WorldChatMessage message)
        {
            await _context.WorldChatMessages.AddAsync(message);
            await _context.SaveChangesAsync();

            await _context.Entry(message).Reference(m => m.Sender).LoadAsync();

            return message;
        }

        public async Task<WorldChatMessage?> GetWorldMessageById(int worldChatMessageId)
        {
            return await _context.WorldChatMessages
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m =>
                    m.WorldChatMessageId == worldChatMessageId &&
                    !m.IsHidden);
        }

        public async Task<WorldChatMessage> UpdateWorldMessage(WorldChatMessage message)
        {
            _context.WorldChatMessages.Update(message);
            await _context.SaveChangesAsync();

            await _context.Entry(message).Reference(m => m.Sender).LoadAsync();

            return message;
        }

        public async Task<(int TotalCount, List<ChatMessage> Items)> GetConversationPaged(
            int playerProfileId,
            int otherPlayerProfileId,
            int page,
            int pageSize)
        {
            page = Math.Max(1, page);
            pageSize = Math.Max(1, pageSize);

            var query = _context.ChatMessages
                .Include(m => m.Sender)
                .Include(m => m.Recipient)
                .Where(m =>
                    !m.IsHidden &&
                    ((m.SenderId == playerProfileId && m.RecipientId == otherPlayerProfileId) ||
                     (m.SenderId == otherPlayerProfileId && m.RecipientId == playerProfileId)))
                .AsNoTracking();

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(m => m.SentAt)
                .ThenByDescending(m => m.ChatMessageId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (totalCount, items);
        }

        public async Task<DateTime?> GetLatestSentAtBySenderId(int senderId)
        {
            return await _context.ChatMessages
                .Where(m => m.SenderId == senderId)
                .OrderByDescending(m => m.SentAt)
                .Select(m => (DateTime?)m.SentAt)
                .FirstOrDefaultAsync();
        }

        public async Task<ChatMessage> Create(ChatMessage message)
        {
            await _context.ChatMessages.AddAsync(message);
            await _context.SaveChangesAsync();

            await _context.Entry(message).Reference(m => m.Sender).LoadAsync();
            await _context.Entry(message).Reference(m => m.Recipient).LoadAsync();

            return message;
        }

        public async Task<ChatMessage?> GetMessageById(int chatMessageId)
        {
            return await _context.ChatMessages
                .Include(m => m.Sender)
                .Include(m => m.Recipient)
                .FirstOrDefaultAsync(m =>
                    m.ChatMessageId == chatMessageId &&
                    !m.IsHidden);
        }

        public async Task<ChatMessage> Update(ChatMessage message)
        {
            _context.ChatMessages.Update(message);
            await _context.SaveChangesAsync();

            await _context.Entry(message).Reference(m => m.Sender).LoadAsync();
            await _context.Entry(message).Reference(m => m.Recipient).LoadAsync();

            return message;
        }
        public async Task<int> DeleteConversation(int firstPlayerProfileId, int secondPlayerProfileId)
        {
            return await _context.ChatMessages
                .Where(m =>
                    (m.SenderId == firstPlayerProfileId && m.RecipientId == secondPlayerProfileId) ||
                    (m.SenderId == secondPlayerProfileId && m.RecipientId == firstPlayerProfileId))
                .ExecuteDeleteAsync();
        }
    }
}
