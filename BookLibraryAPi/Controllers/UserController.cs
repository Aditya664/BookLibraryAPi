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
            // Check if username or email already exists
            var existingUserByEmail = await userManager.FindByEmailAsync(registerRequestDto.Email);
            if (existingUserByEmail != null)
                return BadRequest(ApiResponse<string>.ErrorResponse("Email is already registered."));

            var existingUserByName = await userManager.FindByNameAsync(registerRequestDto.UserName);
            if (existingUserByName != null)
                return BadRequest(ApiResponse<string>.ErrorResponse("Username is already taken."));

            var applicationUser = new ApplicationUser
            {
                UserName = registerRequestDto.UserName,
                Email = registerRequestDto.Email,
                FullName = registerRequestDto.FullName,
            };

            var identityResult = await userManager.CreateAsync(applicationUser, registerRequestDto.Password);
            if (!identityResult.Succeeded)
            {
                // Collect all errors to return
                var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                return BadRequest(ApiResponse<string>.ErrorResponse(errors));
            }

            // Assign roles if provided
            if (registerRequestDto.Roles != null && registerRequestDto.Roles.Any())
            {
                var roleResult = await userManager.AddToRolesAsync(applicationUser, registerRequestDto.Roles);
                if (!roleResult.Succeeded)
                {
                    var roleErrors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    return BadRequest(ApiResponse<string>.ErrorResponse(roleErrors));
                }
            }

            return Ok(ApiResponse<string>.SuccessResponse(null, "User registered successfully! Please login."));
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = userManager.Users.ToList();

            var userDtos = new List<UserListDto>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                userDtos.Add(new UserListDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            return Ok(ApiResponse<List<UserListDto>>.SuccessResponse(userDtos, "Users fetched successfully."));
        }

        // Delete a user by Id (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(ApiResponse<string>.ErrorResponse("User not found."));

            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(ApiResponse<string>.ErrorResponse(errors));
            }

            return Ok(ApiResponse<string>.SuccessResponse(null, "User deleted successfully."));
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = await userManager.FindByEmailAsync(loginRequestDto.UserName);
            if (user == null)
                return BadRequest(ApiResponse<string>.ErrorResponse("Username or Password incorrect."));

            var checkPasswordResult = await userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            if (!checkPasswordResult)
                return BadRequest(ApiResponse<string>.ErrorResponse("Username or Password incorrect."));

            var roles = await userManager.GetRolesAsync(user);
            var jwtToken = tokenRepository.CreateJwtToken(user, roles.ToList());

            var loginResponse = new LoginResponseDto
            {
                JwtToken = jwtToken,
            };

            return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(loginResponse, "Login successful."));
        }
  
}
}
