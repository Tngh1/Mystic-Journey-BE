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

            // Đếm trên query CHƯA Include: COUNT không cần join sang Sender, và bản đếm
            // này chỉ chạm index (IsHidden, SentAt, Id) thay vì phải join cả bảng profile.
            var filtered = _context.WorldChatMessages
                .Where(m => !m.IsHidden)
                .AsNoTracking();

            var totalCount = await filtered.CountAsync();
            var items = await filtered
                .Include(m => m.Sender)
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

            // Caller thường đã nạp sender trong cùng request (EnsurePlayerExists), lúc đó
            // EF fixup gán sẵn navigation nên không cần thêm round-trip.
            if (message.Sender == null)
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

            // message đến từ GetWorldMessageById (đã Include Sender) nên thường bỏ qua được.
            if (message.Sender == null)
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

            var filtered = _context.ChatMessages
                .Where(m =>
                    !m.IsHidden &&
                    ((m.SenderId == playerProfileId && m.RecipientId == otherPlayerProfileId) ||
                     (m.SenderId == otherPlayerProfileId && m.RecipientId == playerProfileId)))
                .AsNoTracking();

            var totalCount = await filtered.CountAsync();
            var items = await filtered
                .Include(m => m.Sender)
                .Include(m => m.Recipient)
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

            // Chỉ nạp navigation nào EF fixup chưa gán sẵn từ entity đang tracked.
            if (message.Sender == null)
                await _context.Entry(message).Reference(m => m.Sender).LoadAsync();
            if (message.Recipient == null)
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

            // message đến từ GetMessageById (đã Include cả 2) nên thường bỏ qua được.
            if (message.Sender == null)
                await _context.Entry(message).Reference(m => m.Sender).LoadAsync();
            if (message.Recipient == null)
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
