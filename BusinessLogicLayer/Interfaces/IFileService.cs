using Microsoft.AspNetCore.Mvc;
using RealTimeChatApi.BusinessLogicLayer.DTOs;

namespace RealTimeChatApi.BusinessLogicLayer.Interfaces
{
    public interface IFileService
    {
        Task<IActionResult> SendFile(SendFileRequestDto request);
        Task<IActionResult> GetFiles(ReceiveFilesRequestDto request);

        Task<IActionResult> DownloadFile(DownloadFileRequestDto request);
    }
}
