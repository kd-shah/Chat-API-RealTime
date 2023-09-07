using RealTimeChatApi.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using RealTimeChatApi.DataAccessLayer.Models;
using RealTimeChatApi.BusinessLogicLayer.Services;
using RealTimeChatApi.BusinessLogicLayer.Interfaces;
using RealTimeChatApi.BusinessLogicLayer.DTOs;
using Microsoft.AspNetCore.Identity;

namespace RealTimeChatApi.Controllers
{
    [Route("api/")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly IUserService _userService;
        private readonly UserManager<IdentityUSer> _userManager;
        private readonly SignInManager<IdentityUSer> _signInManager;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterRequestDto UserObj)
        {
          
            return await _userService.RegisterUserAsync(UserObj);

        }


    }
}
