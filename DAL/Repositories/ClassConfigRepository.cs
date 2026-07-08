using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class ClassConfigRepository : IClassConfigRepository
    {
        private readonly MysticJourneyDbContext _context;

        public ClassConfigRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClassConfig>> GetAll()
        {
            return await _context.ClassConfigs.ToListAsync();
        }

        public async Task<ClassConfig?> GetByClassName(string className)
        {
            return await _context.ClassConfigs
                .FirstOrDefaultAsync(c => c.ClassName.ToLower() == className.ToLower());
        }
    }
}
