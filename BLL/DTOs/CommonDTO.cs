using System.Collections.Generic;

namespace BLL.DTOs
{
    // Initializes a new default instance of the DashboardStatsDto class.
    public class DashboardStatsDto
    {
        // Executes total players operation.
        public int TotalPlayers { get; set; }
        // Executes total accounts operation.
        public int TotalAccounts { get; set; }
        // Executes online players operation.
        public int OnlinePlayers { get; set; }
        // Executes offline players operation.
        public int OfflinePlayers { get; set; }
        // Executes total items operation.
        public int TotalItems { get; set; }
        // Executes total monsters operation.
        public int TotalMonsters { get; set; }
        // Executes total transactions operation.
        public int TotalTransactions { get; set; }
        // Executes total revenue operation.
        public decimal TotalRevenue { get; set; }
    }
}
