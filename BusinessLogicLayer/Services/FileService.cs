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
                    //senderId = authenticatedUser.Id,
                    sender = authenticatedUser,
                    receiver = receiver,
                    filePath = filePath,
                    uploadDateTime = DateTime.Now,
                    isRead = false,
                };

                var message = new Message
                {
                    sender = authenticatedUser,
                    receiver = receiver,
                    content = request.file.FileName,
                    timestamp = DateTime.Now,
                    isRead = false,
                    IsFile = true,
                };

                await _fileRepository.SendFile(fileMetaData);
                await _messageRepository.SendMessage(message);

                return new OkObjectResult(new { File = fileMetaData });
            }
            catch (Exception ex) {
                return new BadRequestObjectResult(new {Message = "Error Occured" });
            }
        } 

        public async Task<IActionResult> GetFiles(ReceiveFilesRequestDto request)
        {
            var authenticatedUser = await _userRepository.GetCurrentUser();
            var receiver = await _messageRepository.GetReceiver(request.receiverId);

            var files = await _fileRepository.GetFiles(receiver, authenticatedUser);

            return new OkObjectResult(files);
        }

        
    }
}
