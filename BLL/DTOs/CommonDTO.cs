using System;

namespace BLL.DTOs
{
    // ============ Dashboard Stats ============
    public class DashboardStatsDto
    {
        public int TotalPlayers { get; set; }
        public int TotalAccounts { get; set; }
        public int TotalItems { get; set; }
        public int TotalMonsters { get; set; }
        public int TotalTransactions { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<MonthlyStatDto> MonthlyStats { get; set; } = new();
    }

    public class MonthlyStatDto
    {
        public string Month { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Amount { get; set; }
    }

    // ============ Paginated Response ============
    public class PaginatedResponseDto<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
