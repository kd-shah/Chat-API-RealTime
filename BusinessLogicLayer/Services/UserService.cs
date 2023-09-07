using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using RealTimeChatApi.DataAccessLayer.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using RealTimeChatApi.DataAccessLayer.Models;
using RealTimeChatApi.BusinessLogicLayer.DTOs;
using Microsoft.AspNetCore.Identity;

namespace RealTimeChatApi.BusinessLogicLayer.Services
{
    public class UserService
    {
        public readonly RealTimeChatDbContext _authContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(RealTimeChatDbContext chatDbContext, IHttpContextAccessor httpContextAccessor, 
            UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _authContext = chatDbContext;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterRequestDto UserObj)
        {

            if (UserObj == null)
                return new BadRequestObjectResult(new { Message = "Invalid request" });

            if (!IsValidEmail(UserObj.email))
                return new BadRequestObjectResult(new { Message = "Invalid email format" });

            if (!IsValidPassword(UserObj.password))
                return new BadRequestObjectResult(new { Message = "Invalid password format" });

            // Check if the user already exists
            var existingUser = await _userManager.FindByEmailAsync(UserObj.email);
            if (existingUser != null)
                return new ConflictObjectResult(new { message = "Registration failed because the email is already registered" });


            var newUser = new IdentityUser
            {
                UserName = UserObj.name,
                Email = UserObj.email,
                //password = UserObj.password,
                //Token = ""

            };

            var result = await _userManager.CreateAsync(newUser, UserObj.password);

            if (result.Succeeded)
            {
                // User registration successful
                return new OkObjectResult(new { Message = "User Registered" });
            }
            else
            {
                // User registration failed
                return new BadRequestObjectResult(new { Message = "User registration failed", Errors = result.Errors });
            }
        }

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
        }

        private bool IsValidPassword(string password)
        {
            int requiredLength = 8;
            if (password.Length < requiredLength)
                return false;

            return true;
        }

        //private IdentityUser GetCurrentLoggedInUser()
        //{
        //    var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        //    if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        //    {
        //        var currentUser = _authContext.Users.FirstOrDefault(u => u.userId == userId);
        //        return currentUser;
        //    }

        //    return null;
        //}
    }
}
