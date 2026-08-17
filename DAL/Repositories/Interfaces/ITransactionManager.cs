using System;
using System.Data;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    // Initializes a new default instance of the ITransactionManager class.
    public interface ITransactionManager
    {
        Task ExecuteInTransactionAsync(Func<Task> action);

        Task ExecuteInTransactionAsync(Func<Task> action, IsolationLevel isolationLevel);

        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action);

        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action, IsolationLevel isolationLevel);
    }
}
