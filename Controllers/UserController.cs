using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RealTimeChatApi.DataAccessLayer.Models;
using RealTimeChatApi.BusinessLogicLayer.Interfaces;
using RealTimeChatApi.BusinessLogicLayer.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Cors;

namespace RealTimeChatApi.Controllers
{
    [Route("api/")]
    [ApiController]
    //[EnableCors("SwaggerPolicy")]
    public class UserController : ControllerBase
    {
        public readonly IUserService _userService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpPost("GoogleAuthenticate")]
        public async Task<Object> GoogleAuthenticate([FromBody] ExternalAuthRequestDto request)
        {
            return await _userService.AuthenticateGoogle(request);
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterRequestDto UserObj)
        {

            return await _userService.RegisterUserAsync(UserObj);

        }

        [HttpPost("login")]
        public async Task<IActionResult> Authenticate([FromBody] LoginRequestDto UserObj)
        {
            return await _userService.Authenticate(UserObj);
        }

        [Authorize]
        [HttpGet("users")]
        public async Task<IEnumerable<AppUser>> GetAllUsers()
        {

            return await _userService.GetAllUsers();
        }

    }
}
