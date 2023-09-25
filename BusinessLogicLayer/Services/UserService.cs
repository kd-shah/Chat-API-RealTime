using Microsoft.IdentityModel.Tokens;
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
using static Google.Apis.Auth.GoogleJsonWebSignature;
using Google.Apis.Auth;

namespace RealTimeChatApi.BusinessLogicLayer.Services
{
    public class UserService : IUserService
    {


        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private static readonly Dictionary<string, string> Users = new Dictionary<string, string>();

        public UserService(IUserRepository userRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {

            _configuration = configuration;
            _userRepository = userRepository;
        }


        public async Task<IActionResult> AuthenticateGoogle([FromBody] ExternalAuthRequestDto request)
        {
            var user = await AuthenticateGoogleUserAsync(request);

            if (user == null)
            {
                // Handle the case where user authentication fails
                return new BadRequestObjectResult(new { Message = "Google authentication failed" });
            }

            var token = CreateToken(user);

            return new OkObjectResult(new
            {
                token = token,
                user = user
            });
        }

        public async Task<AppUser> AuthenticateGoogleUserAsync(ExternalAuthRequestDto request)
        {
            try
            {
                Payload payload = await GoogleJsonWebSignature.ValidateAsync(request.idToken, new ValidationSettings
                {
                    Audience = new[] { _configuration["Google:ClientId"] }
                });

                return await GetOrCreateExternalLoginUser(ExternalAuthRequestDto.PROVIDER, payload.Subject, payload.Email, payload.GivenName, payload.FamilyName);
            }
            catch (InvalidJwtException ex)
            {
                throw;
            }
        }


        private async Task<AppUser> GetOrCreateExternalLoginUser(string provider, string key, string email, string firstName, string lastName)
        {
            var user = await _userRepository.FindByLoginAsync(provider, key);

            if (user != null)
            {
                return user;

            }


            var existingUser = await _userRepository.CheckExistingEmail(email);


            if (existingUser == null)
            {
                // If the email is not found, create a new user
                user = new AppUser
                {
                    Email = email,
                    UserName = email,
                    Id = key,
                    Token = "",
                    Name = firstName,
                };

                var result = await _userRepository.RegisterGoogleUser(user);

                if (!result.Succeeded)
                {
                    return null;
                }
            }

            // Add the external login information to the user
            var info = new UserLoginInfo(provider, key, provider.ToUpperInvariant());

            var addLoginResult = await _userRepository.AuthenticateGoogleUser(user, info);

            if (addLoginResult.Succeeded)
                return user;

            return null;
        }


        // 





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
            if (existingUser != null)
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


            var user = await _userRepository.CheckEmail(UserObj);

            if (user == null)
                return new NotFoundObjectResult(new { Message = "Login failed due to incorrect credentials" });

            var result = await _userRepository.Authenticate(user, UserObj);

            if (result.Succeeded)
            {
                // Authentication succeeded, you can generate a token or return additional user information here.
                var response = new LoginResponseDto
                {
                    userId = user.Id,
                    name = user.Name,
                    email = user.Email,
                    token = CreateToken(user)
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

        public async Task<IEnumerable<AppUser>> GetAllUsers()
        {
            var currentUser = await GetCurrentUser();

            if (currentUser.Id == null)
            {
                throw new Exception("Unable to retrieve current user.");
            }

            var userList = await _userRepository.GetAllUsers(currentUser.Id);


            return userList;
        }

        
        public async Task<AppUser> GetCurrentUser()
        {
            return await _userRepository.GetCurrentUser();
        }

        private string CreateJwt(AppUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("It Is A Secret Key Which Should Not Be Shared With Other Users.....");
            //var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

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

        private string CreateToken(AppUser user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var token = new JwtSecurityToken
                (
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;

        }









    }
}

