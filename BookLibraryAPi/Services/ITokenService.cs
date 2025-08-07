using BookLibraryAPi.Model;
using Microsoft.AspNetCore.Identity;

namespace BookLibraryAPi.Services
{
    public interface ITokenService
    {
        string CreateJwtToken(ApplicationUser user, List<string> roles);

    }
}
