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
using RealTimeChatApi.BusinessLogicLayer.Interfaces;
using RealTimeChatApi.DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Authentication;
using System;

namespace RealTimeChatApi.BusinessLogicLayer.Services
{
    public class UserService : IUserService
    {
       
        
        private readonly IUserRepository _userRepository;
        public UserService( IUserRepository userRepository)
        {
                           
           
            _userRepository = userRepository;
        }

        public async Task<IActionResult> GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse"),
            };

            return Challenge(properties, "Google");
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync("Google");

            if (!authenticateResult.Succeeded)
            {
                return new UnauthorizedObjectResult("Unauthorized");
            }

            // Access user information from authenticateResult.Principal
            var userId = authenticateResult.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userEmail = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var userPassword = authenticateResult.Principal.FindFirst(ClaimTypes.assword)?.Value;

            // Check if the user already exists in your database
            var existingUser = await _userRepository.CheckExistingEmail(userEmail);

            if (existingUser == null)
            {
                // If the user doesn't exist, create a new user in your database
                var newUser = new RegisterRequestDto
                {
                    email = userEmail,
                    // Set other user properties as needed
                };

                var registrationResult = await _userRepository.RegisterUserAsync(newUser);

                if (registrationResult is OkObjectResult registrationOkResult)
                {
                    // Registration successful, you can log in the user here if needed
                    return new OkObjectResult(new { Message = "Google login and registration successful", newUser = registrationOkResult.Value });
                }
                else
                {
                    // Handle registration failure
                    return registrationResult;
                }
            }
            else
            {
                // User already exists, you can log in the user here if needed
                return new OkObjectResult(new { Message = "Google login successful", UserId = existingUser.Id });
            }
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
            var existingUser = await _userRepository.CheckExistingEmail(UserObj.email);
            
            //var existingUser = await _userManager.FindByEmailAsync(UserObj.email);
            if (existingUser)
                return new ConflictObjectResult(new { message = "Registration failed because the email is already registered" });


            var newUser = new AppUser
            {
                Name = UserObj.name,
                UserName = UserObj.email,
                Email = UserObj.email,
                Token = ""

            };

            //var result = await _userManager.CreateAsync(newUser, UserObj.password);
            var result = await _userRepository.RegisterUserAsync(newUser, UserObj);

            if (result.Succeeded)
            {
                
                return new OkObjectResult(new { Message = "User Registered", newUser });
            }
            else
            {
                return new BadRequestObjectResult(new { Message = "User registration failed", Errors = result.Errors });
            }
        }


        public async Task<IActionResult> Authenticate([FromBody] LoginRequestDto UserObj)
        {
            if (UserObj == null)
                return new BadRequestObjectResult(new { Message = "Invalid request" });

            if (!IsValidEmail(UserObj.email))
                return new BadRequestObjectResult(new { Message = "Invalid email format" });

            // Use UserManager to find the user by email
            var user = await _userRepository.CheckEmail(UserObj);
            //var user = await _userManager.FindByEmailAsync(UserObj.email);
            if (user == null)
                return new NotFoundObjectResult(new { Message = "Login failed due to incorrect credentials" });

            // Use SignInManager to check the user's password
            //var result = await _signInManager.CheckPasswordSignInAsync(user, UserObj.password, lockoutOnFailure: false);
            var result = await _userRepository.Authenticate(user, UserObj);

            if (result.Succeeded)
            {
                // Authentication succeeded, you can generate a token or return additional user information here.
                var response = new LoginResponseDto
                {
                    name = user.Name,
                    email = user.Email,
                    token = CreateJwt(user)
                };

                // Generate a token or perform any other post-authentication logic

                return new OkObjectResult(new
                {
                    Message = "Login Success",
                    UserInfo = response, 
                });
            }
            else
            {
                // Authentication failed
                return new BadRequestObjectResult(new
                {
                    Message = "Incorrect Password or Invalid Credentials"
                });
            }

        }

        public async Task<IActionResult> GetAllUsers()
        {
            var currentUser = await GetCurrentUser();

            if (currentUser.Id == null)
            {
                return new BadRequestObjectResult(new { Message = "Unable to retrieve current user." });
            }

            var userList = await _userRepository.GetAllUsers(currentUser.Id);


            return new OkObjectResult(new { users = userList });
        }

        public async Task<AppUser> GetCurrentUser()
        {
            return await _userRepository.GetCurrentUser();
        }

        private string CreateJwt(AppUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("It Is A Secret Key Which Should Not Be Shared With Other Users.....");

            var claims = new List<Claim>
    {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
    };
            var identity = new ClaimsIdentity(claims);


            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials,
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            return jwtTokenHandler.WriteToken(token);
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


    }
}
