  using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RealTimeChatApi.BusinessLogicLayer.DTOs;
using RealTimeChatApi.BusinessLogicLayer.Interfaces;
using RealTimeChatApi.DataAccessLayer.Interfaces;
using RealTimeChatApi.DataAccessLayer.Models;
using RealTimeChatApi.Hubs;
using File = System.IO.File;
using FileModel = RealTimeChatApi.DataAccessLayer.Models.File;


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

            if (request.files == null || request.receiverId == null)
            {
                return new BadRequestObjectResult(new {Message = "Invalid"});
            }

            try
            {

                var message = new Message
                {
                    sender = authenticatedUser,
                    receiver = receiver,
                    timestamp = DateTime.Now,
                    content = request.caption,
                    isRead = false,
                    IsFile = true,
                };

                await _messageRepository.SendMessage(message);

                foreach (var file in request.files)
                {
                    var response = await _fileRepository.SaveFilesLocally(file);
                    var filePath = response?["FilePath"];
                    var uniqueFileName = response?["UniqueFileName"];

                    var fileMetaData = new FileModel
                    {
                        fileName = file.FileName,
                        fileSize = file.Length,
                        contentType = file.ContentType,
                        sender = authenticatedUser,
                        receiver = receiver,
                        filePath = filePath,
                        uploadDateTime = DateTime.Now,
                        isRead = false,
                        messageId = message.messageId,
                        uniqueFileName = uniqueFileName,
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

                    
                }

                return new OkObjectResult(new {Message = "All Files sent"});

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

        public async Task<IActionResult> TextFilePreview(string request)
        {
            var file = await _fileRepository.GetFileByName(request);

            if (file == null)
            {
                return new NotFoundObjectResult(new { message = "File not found" });
            }

            string fileContent = File.ReadAllText(file.filePath);

            return new OkObjectResult(new {fileContent = fileContent });
        }


    }
}
