 using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RealTimeChatApi.BusinessLogicLayer.DTOs;
using RealTimeChatApi.BusinessLogicLayer.Interfaces;
using RealTimeChatApi.DataAccessLayer.Interfaces;
using RealTimeChatApi.DataAccessLayer.Models;
using RealTimeChatApi.Hubs;
using File = RealTimeChatApi.DataAccessLayer.Models.File;


namespace RealTimeChatApi.BusinessLogicLayer.Services
{
    public class FileService : IFileService
    {
        public readonly IUserRepository _userRepository;
        public readonly IFileRepository _fileRepository;
        public readonly IMessageRepository _messageRepository;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly static ConnectionMapping<string> _connections = new ConnectionMapping<string>();
        public FileService(IUserRepository userRepository, IFileRepository fileRepository,
                            IMessageRepository messageRepository, IHubContext<ChatHub> hubContext)
        {
            _userRepository = userRepository;
            _fileRepository = fileRepository;
            _messageRepository = messageRepository;
            _hubContext = hubContext;
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


                var message = new Message
                {
                    sender = authenticatedUser,
                    receiver = receiver,
                    content = request.file.FileName,
                    timestamp = DateTime.Now,
                    isRead = false,
                    IsFile = true,
                };

                await _messageRepository.SendMessage(message);

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
                    messageId = message.messageId,
                };
                await _fileRepository.SendFile(fileMetaData);

                foreach (var connectionId in _connections.GetConnections(message.receiverId))
                {
                    try
                    {
                        await _hubContext.Clients.Client(connectionId).SendAsync("BroadCast", message);

                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine($"Error sending message: {ex.Message}");

                    }

                }



                int fileId = fileMetaData.fileId;

                Message savedMessage = await _messageRepository.FindMessageById(message.messageId);

                if (savedMessage != null)
                {
                    savedMessage.fileId = fileId;
                    await _messageRepository.SaveMessageChanges();

                }



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
