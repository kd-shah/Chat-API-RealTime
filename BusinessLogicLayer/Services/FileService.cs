using Microsoft.AspNetCore.Mvc;
using RealTimeChatApi.BusinessLogicLayer.DTOs;
using RealTimeChatApi.BusinessLogicLayer.Interfaces;
using RealTimeChatApi.DataAccessLayer.Interfaces;
using RealTimeChatApi.DataAccessLayer.Models;
using System.Net.Mail;
using System.Runtime.InteropServices;
using File = RealTimeChatApi.DataAccessLayer.Models.File;


namespace RealTimeChatApi.BusinessLogicLayer.Services
{
    public class FileService : IFileService
    {
        public readonly IUserRepository _userRepository;
        public readonly IFileRepository _fileRepository;
        public readonly IMessageRepository _messageRepository;
        public FileService(IUserRepository userRepository, IFileRepository fileRepository,
                            IMessageRepository messageRepository)
        {
            _userRepository = userRepository;
            _fileRepository = fileRepository;
            _messageRepository = messageRepository;
        }
        public async Task<IActionResult> SendFile(SendFileRequestDto request)
        {
            var authenticatedUser = await _userRepository.GetCurrentUser();
            var receiver = await _messageRepository.GetReceiver(request.receiverId);

            if (request.file == null || request.receiverId == null)
            {
                return new BadRequestObjectResult(new {Message = "Invalid"});
            }

            try
            {
                var filePath = await _fileRepository.SaveFilesLocally(request.file);

               

                var fileMetaData = new File
                {
                    fileName = request.file.FileName,
                    fileSize = request.file.Length,
                    contentType = request.file.ContentType,
                    caption = request.caption,
                    sender = authenticatedUser,
                    receiver = receiver,
                    filePath = filePath,
                    uploadDateTime = DateTime.Now,
                    isRead = false,
                    //Message = message
                };
                await _fileRepository.SendFile(fileMetaData);

                var message = new Message
                {
                    sender = authenticatedUser,
                    receiver = receiver,
                    content = request.file.FileName,
                    timestamp = DateTime.Now,
                    isRead = false,
                    IsFile = true,
                    fileId= fileMetaData.fileId,
                };
                
                await _messageRepository.SendMessage(message);
                

                return new OkObjectResult(new { File = fileMetaData });
            }
            catch (Exception ex) {
                return new BadRequestObjectResult(new {Message = ex });
            }
        } 

        public async Task<IActionResult> GetFiles(ReceiveFilesRequestDto request)
        {
            var authenticatedUser = await _userRepository.GetCurrentUser();
            var receiver = await _messageRepository.GetReceiver(request.receiverId);

            var files = await _fileRepository.GetFiles(receiver, authenticatedUser);

            return new OkObjectResult(files);
        }

        public async Task<IActionResult> DownloadFile(DownloadFileRequestDto request)
        {
            var authenticatedUser = await _userRepository.GetCurrentUser();

            var file = await _fileRepository.GetFileById(request.fileId);

            if (file == null)
            {
                return new NotFoundObjectResult(new { message = "File not found" });
            }

            if (file.senderId != authenticatedUser.Id && file.receiverId != authenticatedUser.Id)
            {
                return new UnauthorizedObjectResult(new { message = "Unauthorized access" });
            }

            var contentType = file.contentType;
            var fileStream = new FileStream(file.filePath, FileMode.Open, FileAccess.Read);

            return new FileStreamResult(fileStream, contentType)
            {
                FileDownloadName = file.fileName
            };

        }


    }
}
