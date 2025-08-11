using BookLibraryAPi.Model;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BookLibraryAPi.DB
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<ReadingProgress> ReadingProgress { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>()
                .HasMany(b => b.Genres)
                .WithMany(g => g.Books);

            modelBuilder.Entity<Genre>().HasData(
             new Genre { Id = 1, Name = "Fantasy", IconName = "sparkles-outline" },
             new Genre { Id = 2, Name = "Science Fiction", IconName = "planet-outline" },
             new Genre { Id = 3, Name = "Romance", IconName = "heart-outline" },
             new Genre { Id = 4, Name = "Thriller", IconName = "flash-outline" },
             new Genre { Id = 5, Name = "Mystery", IconName = "eye-outline" },
             new Genre { Id = 6, Name = "Historical", IconName = "time-outline" },
             new Genre { Id = 7, Name = "Medical", IconName = "medical-outline" }
 );

        }

    }
}
