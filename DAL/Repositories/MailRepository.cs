using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
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

        public async Task DeleteMail(int id)
        {
            var mail = await _context.Mails.FindAsync(id);
            if (mail != null)
            {
                _context.Mails.Remove(mail);
                await _context.SaveChangesAsync();
            }
        }

        public IQueryable<Mail> GetMailsQueryable()
        {
            return _context.Mails
                .Include(m => m.PlayerProfile)
                .Include(m => m.AttachedItem)
                .AsNoTracking();
        }
    }
}
