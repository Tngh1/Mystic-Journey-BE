using DAL.Data;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    /// <summary>
    /// Triển khai quản lý transaction cho cơ sở dữ liệu sử dụng Entity Framework.
    /// Đảm bảo tính nhất quán dữ liệu khi thực hiện nhiều thao tác.
    /// </summary>
    public class TransactionManager : ITransactionManager
    {
        private readonly MysticJourneyDbContext _context;

        public TransactionManager(MysticJourneyDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Thực thi hành động trong một transaction.
        /// Nếu có lỗi xảy ra, transaction sẽ được rollback tự động.
        /// </summary>
        public async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            if (_context.Database.CurrentTransaction != null)
            {
                await action();
                return;
            }

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    await action();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        /// <summary>
        public async Task ExecuteInTransactionAsync(Func<Task> action, IsolationLevel isolationLevel)
        {
            await ExecuteInTransactionAsync(async () =>
            {
                await action();
                return true;
            }, isolationLevel);
        }

        /// Thực thi hành động có返回值 trong transaction.
        /// Nếu có lỗi xảy ra, transaction sẽ được rollback tự động.
        /// </summary>
        public Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action)
            => ExecuteInTransactionAsync(action, IsolationLevel.ReadCommitted);

        public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action, IsolationLevel isolationLevel)
        {
            if (_context.Database.CurrentTransaction != null)
                return await action();

            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(isolationLevel);
                try
                {
                    var result = await action();
                    await transaction.CommitAsync();
                    return result;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }
    }
}
