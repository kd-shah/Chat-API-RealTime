using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using RealTimeChatApi.DataAccessLayer.Data;
using RealTimeChatApi.DataAccessLayer.Interfaces;
using RealTimeChatApi.DataAccessLayer.Models;
using File = RealTimeChatApi.DataAccessLayer.Models.File;

namespace RealTimeChatApi.DataAccessLayer.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly RealTimeChatDbContext _context;

        public FileRepository(RealTimeChatDbContext context)
        {
            _context = context;
        }
        
        public async Task<Dictionary<string, string>> SaveFilesLocally(IFormFile file)
        {
            //var localUploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");

            //if (!Directory.Exists(localUploadsFolder))
            //{
            //    Directory.CreateDirectory(localUploadsFolder);
            //}

            var SharedFiles = "C:/Users/promact/Desktop/Internship/Chat Application RealTime/Frontend/RealTimeChatApp/src/assets/SharedFiles";
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;

            var filePath = Path.Combine(SharedFiles, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }


            var result = new Dictionary<string, string>
                {
                    { "FilePath", filePath },
                     { "UniqueFileName", uniqueFileName }
                };

            return result;

        }
        public async Task<IActionResult> SendFile(File fileMetaData)
        {

            _context.Files.Add(fileMetaData);
            await _context.SaveChangesAsync();

            return null;
        }

        public async Task<IQueryable<File>> GetFiles(AppUser receiver, AppUser authenticatedUser)
        {
            var files = _context.Files.Include(m => m.sender)
               .Include(m => m.receiver)
               .Where(m => (m.senderId == authenticatedUser.Id && m.receiverId == receiver.Id) ||
                           (m.senderId == receiver.Id && m.receiverId == authenticatedUser.Id));

            return files;
        }

        public async Task<File> GetFileById(int fileId)
        {
            var file = _context.Files.FirstOrDefault(f => f.fileId == fileId);
            return file;
        }

       
    }
}
