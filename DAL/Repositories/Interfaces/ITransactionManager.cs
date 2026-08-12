using System;
using System.Data;
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
        /// <summary>Thực thi hành động không trả về giá trị với mức cô lập được chỉ định.</summary>
        Task ExecuteInTransactionAsync(Func<Task> action, IsolationLevel isolationLevel);

        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action);

        /// <summary>Thực thi hành động có返回值 với mức cô lập được chỉ định.</summary>
        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action, IsolationLevel isolationLevel);
    }
}
