using DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mystic_Journey_API.BackgroundJobs
{
    // Executes background service operation.
    public class GuildContributionResetJob : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<GuildContributionResetJob> _logger;

        // Initialize this instance from scope factory and logger and store scope factory and logger for later operations.
        public GuildContributionResetJob(
            IServiceScopeFactory scopeFactory,
            ILogger<GuildContributionResetJob> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        // Executes execute async operation.
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[GuildContributionResetJob] Started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;
                var nextMidnight = now.Date.AddDays(1);
                var delay = nextMidnight - now;

                _logger.LogInformation("[GuildContributionResetJob] Next reset in {Delay}", delay);
                await Task.Delay(delay, stoppingToken);

                if (stoppingToken.IsCancellationRequested) break;

                await ResetDailyContributionAsync();

                if (DateTime.UtcNow.DayOfWeek == DayOfWeek.Monday)
                    await ResetWeeklyContributionAsync();
            }
        }

        // Executes reset daily contribution async operation.
        private async Task ResetDailyContributionAsync()
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<MysticJourneyDbContext>();

                var count = await context.GuildMembers
                    .Where(m => m.DailyContribution > 0)  // Filter records matching the predicate
                    // Apply this bulk change directly in the database without loading every affected entity.
                    .ExecuteUpdateAsync(s => s.SetProperty(m => m.DailyContribution, 0));

                _logger.LogInformation("[GuildContributionResetJob] Daily reset: {Count} members", count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GuildContributionResetJob] Error during daily reset");
            }
        }

        // Executes reset weekly contribution async operation.
        private async Task ResetWeeklyContributionAsync()
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<MysticJourneyDbContext>();

                var count = await context.GuildMembers
                    .Where(m => m.WeeklyContribution > 0)  // Filter records matching the predicate
                    // Apply this bulk change directly in the database without loading every affected entity.
                    .ExecuteUpdateAsync(s => s.SetProperty(m => m.WeeklyContribution, 0));

                _logger.LogInformation("[GuildContributionResetJob] Weekly reset: {Count} members", count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GuildContributionResetJob] Error during weekly reset");
            }
        }
    }
}
