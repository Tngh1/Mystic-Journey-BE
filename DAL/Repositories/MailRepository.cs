using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class MailRepository : IMailRepository
    {
        private readonly MysticJourneyDbContext _context;

        public MailRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<Mail?> GetMailById(int id)
        {
            return await _context.Mails
                .Include(m => m.PlayerProfile)
                .Include(m => m.AttachedItem)
                .FirstOrDefaultAsync(m => m.MailId == id);
        }

        public async Task<List<Mail>> GetMailsByPlayerId(int playerProfileId)
        {
            return await _context.Mails
                .Include(m => m.PlayerProfile)
                .Include(m => m.AttachedItem)
                .Where(m => m.PlayerProfileId == playerProfileId)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<List<Mail>> GetUnreadMailsByPlayerId(int playerProfileId)
        {
            return await _context.Mails
                .Include(m => m.PlayerProfile)
                .Include(m => m.AttachedItem)
                .Where(m => m.PlayerProfileId == playerProfileId && !m.IsRead)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<Mail> CreateMail(Mail mail)
        {
            mail.SentAt = DateTime.UtcNow;
            await _context.Mails.AddAsync(mail);
            await _context.SaveChangesAsync();
            return mail;
        }

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

        public async Task<Mail> UpdateMail(Mail mail)
        {
_context.Mails.Update(mail);
            await _context.SaveChangesAsync();
            return mail;
        }


        public async Task<(int TotalCount, List<Mail> Items)> GetMailsPaged(int page, int pageSize, string? search, bool? isRead, bool? isClaimed)
        {
            var query = _context.Mails
                .Include(m => m.PlayerProfile)
                .Include(m => m.AttachedItem)
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
    }
}
