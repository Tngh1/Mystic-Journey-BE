using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetDashboardStats();
    }
}
