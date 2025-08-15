using BookLibraryAPi.DTOs;
using BookLibraryAPi.Model;
using BookLibraryAPi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BookLibraryAPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ITokenService tokenRepository;
        public UserController(UserManager<ApplicationUser> userManager, ITokenService tokenRepository)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var applicationUser = new ApplicationUser
            {
                UserName = registerRequestDto.UserName,
                Email = registerRequestDto.Email,
                FullName = registerRequestDto.FullName,
            };
            var identityResult = await userManager.CreateAsync(applicationUser, registerRequestDto.Password);
            if (identityResult.Succeeded)
            {
                if (registerRequestDto.Roles != null && registerRequestDto.Roles.Any())
                {
                    identityResult = await userManager.AddToRolesAsync(applicationUser, registerRequestDto.Roles);
                }
                if (identityResult.Succeeded)
                {
                    return Ok(ApiResponse<string>.SuccessResponse(null, "User was registred! Please login"));
                }
            }
            return BadRequest(ApiResponse<string>.ErrorResponse("something went wrong!"));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = await userManager.FindByEmailAsync(loginRequestDto.UserName);
            if (user != null)
            {
                var checkPasswordResult = await userManager.CheckPasswordAsync(user, loginRequestDto.Password);
                if (checkPasswordResult)
                {
                    var roles = await userManager.GetRolesAsync(user);
                    if (roles != null)
                    {
                        var jwtToken = tokenRepository.CreateJwtToken(user, roles.ToList());
                        var loginResponse = new LoginResponseDto
                        {
                            JwtToken = jwtToken,
                        };
                        return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(loginResponse, ""));
                    }
                }
            }
            return BadRequest(ApiResponse<string>.ErrorResponse("Username or Password incorrect"));
        }
   }
}
