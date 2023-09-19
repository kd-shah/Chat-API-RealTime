using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealTimeChatApi.BusinessLogicLayer.DTOs;
using RealTimeChatApi.BusinessLogicLayer.Services;
using RealTimeChatApi.DataAccessLayer.Models;

namespace RealTimeChatApi.BusinessLogicLayer.Interfaces
{
    public interface IUserService
    {
        Task<IActionResult> RegisterUserAsync(RegisterRequestDto UserObj);

        Task<IActionResult> Authenticate(LoginRequestDto UserObj);

        Task<IEnumerable<AppUser>> GetAllUsers();

        Task<IActionResult> AuthenticateGoogle(ExternalAuthRequestDto request);

    }
}
