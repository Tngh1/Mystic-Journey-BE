using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Services.Interfaces
{
    // Initializes a new default instance of the IDashboardService class.
    public interface IDashboardService
    {

        Task<DashboardStatsDto> GetDashboardStats();
    }
}
