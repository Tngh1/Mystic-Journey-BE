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
    public class MailRepository : IMailRepository
    {
        private readonly MysticJourneyDbContext _context;

        public MailRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<Mail?> GetByIdAsync(Guid mailId)
        {
            return await _context.Mails
                .Include(m => m.AttachedItem)
                .FirstOrDefaultAsync(m => m.Id == mailId);
        }

        public async Task<Mail?> GetByIdWithDetailsAsync(Guid mailId)
        {
            return await _context.Mails
                .Include(m => m.AttachedItem)
                .Include(m => m.PlayerProfile)
                .FirstOrDefaultAsync(m => m.Id == mailId);
        }

        public async Task<List<Mail>> GetByPlayerProfileIdAsync(Guid playerProfileId, int pageNumber = 1, int pageSize = 20)
        {
            return await _context.Mails
                .Include(m => m.AttachedItem)
                .Where(m => m.PlayerProfileId == playerProfileId)
                .OrderByDescending(m => m.SentAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Mail>> GetUnreadMailsAsync(Guid playerProfileId)
        {
            return await _context.Mails
                .Include(m => m.AttachedItem)
                .Where(m => m.PlayerProfileId == playerProfileId && !m.IsRead)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(Guid playerProfileId)
        {
            return await _context.Mails
                .Where(m => m.PlayerProfileId == playerProfileId && !m.IsRead)
                .CountAsync();
        }

        public async Task<Mail> CreateAsync(Mail mail)
        {
            await _context.Mails.AddAsync(mail);
            await _context.SaveChangesAsync();
            return mail;
        }

        public async Task<Mail> UpdateAsync(Mail mail)
        {
            _context.Mails.Update(mail);
            await _context.SaveChangesAsync();
            return mail;
        }

        public async Task<int> GetTotalCountAsync(Guid playerProfileId)
        {
            return await _context.Mails
                .Where(m => m.PlayerProfileId == playerProfileId)
                .CountAsync();
        }
    }
}
