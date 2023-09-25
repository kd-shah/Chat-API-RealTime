using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealTimeChatApi.BusinessLogicLayer.DTOs;
using RealTimeChatApi.BusinessLogicLayer.Interfaces;

namespace RealTimeChatApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        public readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
               _fileService = fileService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> SendFile([FromForm] SendFileRequestDto request)
        {
            return await _fileService.SendFile(request);
        }

        [HttpGet("files")]
        public async Task<IActionResult> ReceiveFile([FromQuery]ReceiveFilesRequestDto request)
        {
            return await _fileService.GetFiles(request);
        }

        [HttpGet("download")]
        public async Task<IActionResult> DownloadFile([FromQuery]DownloadFileRequestDto request)
        {
            return await _fileService.DownloadFile(request);
        }
    }
}
