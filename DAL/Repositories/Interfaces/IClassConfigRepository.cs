using DAL.Models;

namespace DAL.Repositories.Interfaces
{
    public interface IClassConfigRepository
    {
        Task<ClassConfig?> GetByClassName(string className);
        Task<IEnumerable<ClassConfig>> GetAll();
    }
}
