using System;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    /// <summary>
    /// Định nghĩa các thao tác quản lý transaction cho cơ sở dữ liệu.
    /// </summary>
    public interface ITransactionManager
    {
        /// <summary>Thực thi hành động trong một transaction, tự động rollback nếu có lỗi.</summary>
        Task ExecuteInTransactionAsync(Func<Task> action);

        /// <summary>Thực thi hành động có返回值 trong transaction, tự động rollback nếu có lỗi.</summary>
        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action);
    }
}
