using Microsoft.AspNetCore.Mvc;
using RealTimeChatApi.DataAccessLayer.Models;

namespace RealTimeChatApi.DataAccessLayer.Interfaces
{
    public class ILogRepository
    {
        Task<IActionResult> GetLogs();
    }
}
