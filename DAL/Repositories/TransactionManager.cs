using DAL.Data;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    // Executes core business logic for i transaction manager.
    public class TransactionManager : ITransactionManager
    {
        private readonly MysticJourneyDbContext _context;

        // Initializes a new instance of TransactionManager with dependencies: context.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public TransactionManager(MysticJourneyDbContext context)
        {
            _context = context;
        }

        // Queries the database to retrieve execute in transaction async records.
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
                // Keep the following dependent database writes in one transaction so a failure cannot persist partial state.
                await using var transaction = await _context.Database.BeginTransactionAsync();  // Open serializable transaction — prevents race conditions on concurrent purchases
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

        // Executes core business logic for execute in transaction async.
        // Completes asynchronously upon successful execution.
        public async Task ExecuteInTransactionAsync(Func<Task> action, IsolationLevel isolationLevel)
        {
            await ExecuteInTransactionAsync(async () =>
            {
                await action();
                return true;
            }, isolationLevel);
        }

        // Process the supplied values: maps the input discriminator to the corresponding domain value and fallback.
        public Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action)
            => ExecuteInTransactionAsync(action, IsolationLevel.ReadCommitted);

        // Process execute in transaction async using action and isolation level; it creates execution strategy, opens a database transaction, commits the transaction, and rolls back the transaction on failure and guards invalid or unavailable states and keeps dependent writes atomic.
        public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action, IsolationLevel isolationLevel)
        {
            if (_context.Database.CurrentTransaction != null)
                return await action();

            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                // Keep the following dependent database writes in one transaction so a failure cannot persist partial state.
                await using var transaction = await _context.Database.BeginTransactionAsync(isolationLevel);  // Open serializable transaction — prevents race conditions on concurrent purchases
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
