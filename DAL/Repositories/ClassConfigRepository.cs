using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    // Queries the database to retrieve i class config repository records.
    public class ClassConfigRepository : IClassConfigRepository
    {
        private readonly MysticJourneyDbContext _context;

        // Initializes a new instance of ClassConfigRepository with dependencies: context.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public ClassConfigRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        // Load all; it materializes the query results.
        public async Task<IEnumerable<ClassConfig>> GetAll()
        {
            return await _context.ClassConfigs.ToListAsync();  // Materialize the query into a list from the database
        }

        // Queries the database to retrieve get by class name records.
        // Returns the matching ClassConfig? entity result or default if not found.
        public async Task<ClassConfig?> GetByClassName(string className)
        {
            return await _context.ClassConfigs
                .FirstOrDefaultAsync(c => c.ClassName.ToLower() == className.ToLower());  // Fetch single matching record or null if not found
        }
    }
}
