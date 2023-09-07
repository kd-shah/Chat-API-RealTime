using Microsoft.AspNetCore.Mvc;
using RealTimeChatApi.BusinessLogicLayer.DTOs;
using RealTimeChatApi.BusinessLogicLayer.Services;

namespace RealTimeChatApi.BusinessLogicLayer.Interfaces
{
    public interface IUserService
    {
        Task<IActionResult> RegisterUserAsync(RegisterRequestDto UserObj);
    }
}
