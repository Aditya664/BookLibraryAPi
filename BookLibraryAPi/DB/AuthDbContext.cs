using BookLibraryAPi.Entities;
using BookLibraryAPi.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookLibraryAPi.DB
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var adminRoleId = "ad40b0ce-8a5e-4e33-8c36-123456789000";
            var userRoleId = "ba12c678-df89-43cb-9876-987654321000";

            var roles = new List<IdentityRole>
    {
        new IdentityRole
        {
            Id = adminRoleId,
            Name = "Admin",
            NormalizedName = "ADMIN",
            ConcurrencyStamp = Guid.NewGuid().ToString()
        },
        new IdentityRole
        {
            Id = userRoleId,
            Name = "User",
            NormalizedName = "USER",
            ConcurrencyStamp = Guid.NewGuid().ToString()
        }
            };

            builder.Entity<IdentityRole>().HasData(roles);
        }

    }
}
