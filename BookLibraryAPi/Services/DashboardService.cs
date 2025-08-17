using BookLibraryAPi.DB;
using BookLibraryAPi.DTOs;
using BookLibraryAPi.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookLibraryAPi.Services
{
    public class DashboardService:IDashboardService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardService(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var totalUsers = await _userManager.Users.CountAsync();
            var totalSubscriptions = await _userManager.Users.CountAsync(u => u.HasSubscription);

            return new DashboardStatsDto
            {
                TotalUsers = totalUsers,
                TotalBooks = await _context.Books.CountAsync(),
                TotalGenres = await _context.Genres.CountAsync(),
                TotalReviews = await _context.Reviews.CountAsync(),
                TotalFavorites = await _context.Favorites.CountAsync(),
                TotalSubscriptions = totalSubscriptions,
                TotalReadingSessions = await _context.ReadingProgress.CountAsync()
            };
        }
    }
}
