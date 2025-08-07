using Microsoft.AspNetCore.Identity;

namespace BookLibraryAPi.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

    }
}
