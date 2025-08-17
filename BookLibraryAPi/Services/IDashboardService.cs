using BookLibraryAPi.DTOs;

namespace BookLibraryAPi.Services
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetDashboardStatsAsync();
    }
}
