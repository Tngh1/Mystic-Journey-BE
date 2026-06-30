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
    public class MailRepository : IMailRepository
    {
        private readonly MysticJourneyDbContext _context;

        public MailRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        // ── Query ──

        /// <summary>Tìm thư theo mã, kèm người nhận và vật phẩm đính kèm.</summary>
        public async Task<Mail?> GetMailById(int id)
        {
            return await _context.Mails
                .Include(m => m.PlayerProfile)
                .Include(m => m.AttachedItem)
                .FirstOrDefaultAsync(m => m.MailId == id);
        }

        /// <summary>Lấy tất cả thư của người chơi, sắp xếp theo thời gian gửi giảm dần.</summary>
        public async Task<List<Mail>> GetMailsByPlayerId(int playerProfileId)
        {
            return await _context.Mails
                .Include(m => m.PlayerProfile)
                .Include(m => m.AttachedItem)
                .Where(m => m.PlayerProfileId == playerProfileId)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();
        }

        /// <summary>Lấy các thư chưa đọc của người chơi.</summary>
        public async Task<List<Mail>> GetUnreadMailsByPlayerId(int playerProfileId)
        {
            return await _context.Mails
                .Include(m => m.PlayerProfile)
                .Include(m => m.AttachedItem)
                .Where(m => m.PlayerProfileId == playerProfileId && !m.IsRead)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();
        }

        // ── CRUD ──

        /// <summary>Tạo thư mới và gửi đến người chơi (tự động ghi nhận thời gian gửi).</summary>
        public async Task<Mail> CreateMail(Mail mail)
        {
            mail.SentAt = DateTime.UtcNow;
            await _context.Mails.AddAsync(mail);
            await _context.SaveChangesAsync();
            return mail;
        }

        /// <summary>Tạo nhiều thư cùng lúc (gửi hàng loạt với cùng thời gian gửi).</summary>
        public async Task<List<Mail>> CreateBulkMails(List<Mail> mails)
        {
            var now = DateTime.UtcNow;
            foreach (var mail in mails)
            {
                mail.SentAt = now;
            }
            await _context.Mails.AddRangeAsync(mails);
            await _context.SaveChangesAsync();
            return mails;
        }

        /// <summary>Cập nhật thông tin thư (đánh dấu đã đọc, đã nhận vật phẩm...).</summary>
        public async Task<Mail> UpdateMail(Mail mail)
        {
            _context.Mails.Update(mail);
            await _context.SaveChangesAsync();
            return mail;
        }

        /// <summary>Xóa mềm thư (đánh dấu đã xóa và ghi nhận thời gian xóa).</summary>
        public async Task<Mail> SoftDeleteMail(int mailId)
        {
            var mail = await _context.Mails.FindAsync(mailId)
                ?? throw new KeyNotFoundException($"Mail with id {mailId} not found.");
            mail.IsDeleted = true;
            mail.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return mail;
        }

        // ── Phân trang ──

        /// <summary>Lấy danh sách thư có phân trang, lọc theo tìm kiếm (tiêu đề), trạng thái đọc và nhận.</summary>
        public async Task<(int TotalCount, List<Mail> Items)> GetMailsPaged(int page, int pageSize, string? search, bool? isRead, bool? isClaimed)
        {
            var query = _context.Mails
                .Include(m => m.PlayerProfile)
                .Include(m => m.AttachedItem)
                .Where(m => !m.IsDeleted)
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

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (totalCount, items);
        }

        /// <summary>Lấy thư của một người chơi cụ thể có phân trang.</summary>
        public async Task<(int TotalCount, List<Mail> Items)> GetMailsByPlayerIdPaged(int playerProfileId, int page, int pageSize)
        {
            var query = _context.Mails
                .Include(m => m.PlayerProfile)
                .Include(m => m.AttachedItem)
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
