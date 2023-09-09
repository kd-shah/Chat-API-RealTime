using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealTimeChatApi.DataAccessLayer.Data;
using RealTimeChatApi.DataAccessLayer.Interfaces;

namespace RealTimeChatApi.BusinessLogicLayer.Services
{
    public class LogService
    {
        private readonly RealTimeChatDbContext _context;
        private readonly ILogRepository _logRepository;
        public LogService(RealTimeChatDbContext context, ILogRepository logRepository)
        {
            _logRepository = logRepository;
            _context = context;
        }

        public async Task<IActionResult> GetLogs([FromQuery] string timeframe, [FromQuery] string startTime = null, [FromQuery] string endTime = null)
        {
            DateTime? parsedStartTime = ParseDateTime(startTime);
            DateTime? parsedEndTime = ParseDateTime(endTime);
            if (parsedStartTime == null)
                parsedStartTime = DateTime.Now.AddMinutes(-5);
            if (parsedEndTime == null)
                parsedEndTime = DateTime.Now;

            Console.WriteLine($"parsedStartTime: {parsedStartTime}, parsedEndTime: {parsedEndTime}");


            switch (timeframe)
            {
                case "5":
                    parsedStartTime = DateTime.Now.AddMinutes(-5);
                    break;
                case "10":
                    parsedStartTime = DateTime.Now.AddMinutes(-10);
                    break;
                case "30":
                    parsedStartTime = DateTime.Now.AddMinutes(-30);
                    break;
                case "custom":
                    parsedStartTime = parsedStartTime ?? DateTime.Now.AddMinutes(-30);
                    parsedEndTime = parsedEndTime ?? DateTime.Now;

                    break;
                default:
                    break;
            }

            //var logs = await _context.Logs
            //    .Where(log => log.timeStamp >= parsedStartTime && log.timeStamp <= parsedEndTime)
            //    .ToListAsync();

            var logs = await _logRepository.GetLogs();
            if (logs.Count == 0)
                return new NotFoundObjectResult("no log found");
            return new OkObjectResult(logs);
        }
        private DateTime? ParseDateTime(string dateTimeString)
        {
            if (string.IsNullOrEmpty(dateTimeString))
                return null;
            if (DateTime.TryParse(dateTimeString, out DateTime parsedDateTime))
                return parsedDateTime;
            return null;
        }
    }
}
