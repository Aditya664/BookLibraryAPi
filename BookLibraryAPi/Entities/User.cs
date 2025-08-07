using Microsoft.AspNetCore.Identity;

namespace BookLibraryAPi.Entities
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
    }
}
