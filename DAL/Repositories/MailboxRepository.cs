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
    /// <summary>
    /// Triển khai các thao tác truy cập dữ liệu cho thư tín trong game sử dụng Entity Framework.
    /// </summary>
    public class MailboxRepository : IMailboxRepository
    {
        private readonly MysticJourneyDbContext _context;

        public MailboxRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        // ── Query ──

        /// <summary>Tìm thư theo mã, kèm người nhận và vật phẩm đính kèm.</summary>
        public async Task<Mailbox?> GetMailboxById(int id)
        {
            return await _context.Mailboxes
                .Include(m => m.PlayerProfile)
                .Include(m => m.AttachedItems).ThenInclude(a => a.Item)
                .FirstOrDefaultAsync(m => m.MailboxId == id);
        }

        /// <summary>Lấy tất cả thư của người chơi, sắp xếp theo thời gian gửi giảm dần.</summary>
        public async Task<List<Mailbox>> GetMailboxesByPlayerId(int playerProfileId)
        {
            return await _context.Mailboxes
                .Include(m => m.PlayerProfile)
                .Include(m => m.AttachedItems).ThenInclude(a => a.Item)
                .Where(m => m.PlayerProfileId == playerProfileId)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();
        }

        /// <summary>Lấy các thư chưa đọc của người chơi.</summary>
        public async Task<List<Mailbox>> GetUnreadMailboxesByPlayerId(int playerProfileId)
        {
            return await _context.Mailboxes
                .Include(m => m.PlayerProfile)
                .Include(m => m.AttachedItems).ThenInclude(a => a.Item)
                .Where(m => m.PlayerProfileId == playerProfileId && !m.IsRead)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();
        }

        // ── CRUD ──

        /// <summary>Tạo thư mới và gửi đến người chơi (tự động ghi nhận thời gian gửi).</summary>
        public async Task<Mailbox> CreateMailbox(Mailbox mailbox)
        {
            mailbox.SentAt = DateTime.UtcNow;
            await _context.Mailboxes.AddAsync(mailbox);
            await _context.SaveChangesAsync();
            return mailbox;
        }

        /// <summary>Tạo nhiều thư cùng lúc (gửi hàng loạt với cùng thời gian gửi).</summary>
        public async Task<List<Mailbox>> CreateBulkMailboxes(List<Mailbox> mailboxes)
        {
            var now = DateTime.UtcNow;
            foreach (var mailbox in mailboxes)
            {
                mailbox.SentAt = now;
            }
            await _context.Mailboxes.AddRangeAsync(mailboxes);
            await _context.SaveChangesAsync();
            return mailboxes;
        }

        /// <summary>Cập nhật thông tin thư (đánh dấu đã đọc, đã nhận vật phẩm...).</summary>
        public async Task<Mailbox> UpdateMailbox(Mailbox mailbox)
        {
            _context.Mailboxes.Update(mailbox);
            await _context.SaveChangesAsync();
            return mailbox;
        }

        /// <summary>Xóa mềm thư (đánh dấu đã xóa và ghi nhận thời gian xóa).</summary>
        public async Task<Mailbox> SoftDeleteMailbox(int mailboxId)
        {
            var mailbox = await _context.Mailboxes.FindAsync(mailboxId)
                ?? throw new KeyNotFoundException($"Mailbox with id {mailboxId} not found.");
            mailbox.IsDeleted = true;
            mailbox.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return mailbox;
        }

        // ── Phân trang ──

        /// <summary>Lấy danh sách thư có phân trang, lọc theo tìm kiếm (tiêu đề), trạng thái đọc và nhận.</summary>
        public async Task<(int TotalCount, List<Mailbox> Items)> GetMailboxesPaged(int page, int pageSize, string? search, bool? isRead, bool? isClaimed, string? sortBy = null, string? sortOrder = null)
        {
            var query = _context.Mailboxes
                .Include(m => m.PlayerProfile)
                .Include(m => m.AttachedItems).ThenInclude(a => a.Item)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Title.Contains(search));
            }
            if (isRead.HasValue)
            {
                query = query.Where(x => x.IsRead == isRead.Value);
            }
            if (isClaimed.HasValue)
            {
                query = query.Where(x => x.IsClaimed == isClaimed.Value);
            }

            bool desc = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);
            query = (sortBy?.ToLowerInvariant()) switch
            {
                "title" => desc ? query.OrderByDescending(x => x.Title) : query.OrderBy(x => x.Title),
                "sentat" => desc ? query.OrderByDescending(x => x.SentAt) : query.OrderBy(x => x.SentAt),
                "expiresat" => desc ? query.OrderByDescending(x => x.ExpiredAt) : query.OrderBy(x => x.ExpiredAt),
                "isread" => desc ? query.OrderByDescending(x => x.IsRead) : query.OrderBy(x => x.IsRead),
                "isclaimed" => desc ? query.OrderByDescending(x => x.IsClaimed) : query.OrderBy(x => x.IsClaimed),
                _ => desc ? query.OrderByDescending(x => x.MailboxId) : query.OrderBy(x => x.MailboxId),
            };

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (totalCount, items);
        }

        /// <summary>Lấy thư của một người chơi cụ thể có phân trang.</summary>
        public async Task<(int TotalCount, List<Mailbox> Items)> GetMailboxesByPlayerIdPaged(int playerProfileId, int page, int pageSize)
        {
            var query = _context.Mailboxes
                .Include(m => m.PlayerProfile)
                .Include(m => m.AttachedItems).ThenInclude(a => a.Item)
                .Where(m => m.PlayerProfileId == playerProfileId && !m.IsDeleted)
                .AsNoTracking();

            int totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(m => m.SentAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (totalCount, items);
        }
    }
}
