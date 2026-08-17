using DAL.Models;

namespace DAL.Repositories.Interfaces
{
    // Initializes a new default instance of the IClassConfigRepository class.
    public interface IClassConfigRepository
    {
        Task<ClassConfig?> GetByClassName(string className);
        Task<IEnumerable<ClassConfig>> GetAll();
    }
}
