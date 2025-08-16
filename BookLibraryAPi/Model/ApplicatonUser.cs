using Microsoft.AspNetCore.Identity;

namespace BookLibraryAPi.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public double TotalReadingHours { get; set; } = 0;

        public bool HasSubscription { get; set; } = false;

    }
}
