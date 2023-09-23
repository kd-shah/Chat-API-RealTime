using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using RealTimeChatApi.BusinessLogicLayer.DTOs;
using RealTimeChatApi.DataAccessLayer.Models;
using File = RealTimeChatApi.DataAccessLayer.Models.File;

namespace RealTimeChatApi.DataAccessLayer.Interfaces
{
    public interface IFileRepository
    {
        Task<string> SaveFilesLocally(IFormFile file);

        Task<IActionResult> SendFile(File fileMetaData);

        Task<IQueryable<File>> GetFiles(AppUser receiver, AppUser authenticatedUser);

        Task<File>GetFileById(int fileId);
    }
}
