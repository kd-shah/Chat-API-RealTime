using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealTimeChatApi.DataAccessLayer.Data;
using RealTimeChatApi.DataAccessLayer.Interfaces;

namespace RealTimeChatApi.DataAccessLayer.Repositories
{
    public class LogRepository
    {
        private readonly RealTimeChatDbContext _context;
       
        public LogRepository(RealTimeChatDbContext context)
        {
            
            _context = context;
        }
        public async Task<> GetLogs() {
            var logs = _context.Logs;
            return logs;
        }
    }
}
