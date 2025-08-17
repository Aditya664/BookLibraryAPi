namespace BookLibraryAPi.DTOs
{
    public class DashboardStatsDto
    {
        public int TotalUsers { get; set; }
        public int TotalBooks { get; set; }
        public int TotalGenres { get; set; }
        public int TotalReviews { get; set; }
        public int TotalFavorites { get; set; }
        public int TotalSubscriptions { get; set; }
        public int TotalReadingSessions { get; set; }
    }
}
