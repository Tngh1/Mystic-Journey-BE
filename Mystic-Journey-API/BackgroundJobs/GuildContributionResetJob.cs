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
    /// <summary>
    /// Resets GuildMember contribution counters:
    ///   - DailyContribution: reset every day at 00:00 UTC
    ///   - WeeklyContribution: reset every Monday at 00:00 UTC
    /// </summary>
    public class GuildContributionResetJob : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<GuildContributionResetJob> _logger;

        public GuildContributionResetJob(
            IServiceScopeFactory scopeFactory,
            ILogger<GuildContributionResetJob> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[GuildContributionResetJob] Started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;
                // Calculate next midnight UTC
                var nextMidnight = now.Date.AddDays(1);
                var delay = nextMidnight - now;

                _logger.LogInformation("[GuildContributionResetJob] Next reset in {Delay}", delay);
                await Task.Delay(delay, stoppingToken);

                if (stoppingToken.IsCancellationRequested) break;

                await ResetDailyContributionAsync();

                // If Monday, also reset weekly
                if (DateTime.UtcNow.DayOfWeek == DayOfWeek.Monday)
                    await ResetWeeklyContributionAsync();
            }
        }

        private async Task ResetDailyContributionAsync()
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<MysticJourneyDbContext>();

                var count = await context.GuildMembers
                    .Where(m => m.DailyContribution > 0)
                    .ExecuteUpdateAsync(s => s.SetProperty(m => m.DailyContribution, 0));

                _logger.LogInformation("[GuildContributionResetJob] Daily reset: {Count} members", count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GuildContributionResetJob] Error during daily reset");
            }
        }

        private async Task ResetWeeklyContributionAsync()
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<MysticJourneyDbContext>();

                var count = await context.GuildMembers
                    .Where(m => m.WeeklyContribution > 0)
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
