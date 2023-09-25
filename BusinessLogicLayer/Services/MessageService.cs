using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RealTimeChatApi.BusinessLogicLayer.DTOs;
using RealTimeChatApi.BusinessLogicLayer.Interfaces;
using RealTimeChatApi.DataAccessLayer.Interfaces;
using RealTimeChatApi.DataAccessLayer.Models;
using RealTimeChatApi.Hubs;


namespace RealTimeChatApi.BusinessLogicLayer.Services
{
    public class MessageService : IMessageService
    {
        
        public readonly IMessageRepository _messageRepository;
        public readonly IUserRepository _userRepository;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly static ConnectionMapping<string> _connections = new ConnectionMapping<string>();

        public MessageService( IMessageRepository messageRepository, IHubContext<ChatHub> hubContext,
                               IUserRepository userRepository)
        {
            
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> SendMessage(string receiverId, [FromBody] SendMessageRequestDto request)
        {
            if(request == null)
            {
                return new BadRequestObjectResult(new { Message = "Message cannot be blank" });
            }


            //var senderId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var sender = await _userRepository.GetCurrentUser();
            if (sender == null)
            {
                return new NotFoundObjectResult("Sender not found");
            }


            //var receiver = await _context.Users.FindAsync(receiverId);
            var receiver = await _messageRepository.GetReceiver(receiverId);
            if (receiver == null)
            {
                return new NotFoundObjectResult("Receiver not found");
            }

            var content = request.content;
            var timestamp = DateTime.Now;

            var message = new Message
            {
                sender = sender,
                receiver = receiver,
                content = content,
                timestamp = timestamp,
                isRead = false,
                IsFile = false,
            };

            await _messageRepository.SendMessage(message);
            

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

            var response = new 
            {
                messageId = message.messageId,
                senderId = sender.Id,
                receiverId = receiver.Id,
                content = content,
                timestamp = timestamp,
                isRead = false,
                isFile = false,
            };

            return new OkObjectResult(response);
        }

        
        public async Task<IActionResult> EditMessage(int messageId, [FromBody] EditMessageRequestDto request)
        {
            var authenticatedUser = await _userRepository.GetCurrentUser();

            if (request == null)
            {
                return new NotFoundObjectResult("Message Not Found");

            }

            var message = await _messageRepository.GetMessage(messageId);

            if (message == null)
            {
                return new NotFoundObjectResult("Message Not Found");
            }

            if (message.senderId != authenticatedUser.Id)
            {
                return new UnauthorizedObjectResult("Unauthorized");
            }
            message.content = request.content;

            //await _context.SaveChangesAsync();
            await _messageRepository.EditMessage();

            foreach (var connectionId in _connections.GetConnections(message.receiverId))
            {
                try
                {
                    await _hubContext.Clients.Client(connectionId).SendAsync("BroadCast", message);

                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine($"Error sending message: {ex.Message}");
                    // Handle the exception or take appropriate action
                }

            }

            return new  OkObjectResult(new { message = "Message edited successfully" });
        }
    
        public async Task<IActionResult>DeleteMessage(int messageId)
        {
            var authenticatedUser = await _userRepository.GetCurrentUser();

            if (messageId == null)
            {
                return new NotFoundObjectResult("Message Not Found");

            }

            var message = await _messageRepository.GetMessage(messageId);

            if (message.senderId != authenticatedUser.Id)
            {
                return new UnauthorizedObjectResult("Unauthorized");
            }

            await _messageRepository.DeleteMessage(message);

            return new OkObjectResult(new { message = "Message Deleted successfully" });
        }

        
        
        public async Task<IActionResult> SearchConversations(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new BadRequestObjectResult("Invalid keyword");
            }

            var user = await _userRepository.GetCurrentUser();

            if (string.IsNullOrEmpty(user.Id))
            {
                return new UnauthorizedObjectResult("Unauthorized");
            }

            var matchingMessages = await _messageRepository.SearchMessages(user.Id, query);

            if (matchingMessages == null || !matchingMessages.Any())
            {
                return new OkObjectResult(new List<object>()); 
            }

            var response = matchingMessages.Select(message => new
            {
                id = message.messageId,
                senderId = message.sender.Id,
                receiverId = message.receiver.Id,
                content = message.content,
                timestamp = message.timestamp,
                isRead = message.isRead,
            });

            return new OkObjectResult(response);
        }

        public async Task<IActionResult> GetAllUnReadMessages()
        {
            var authenticatedUser = await _userRepository.GetCurrentUser();

            var unReadMessages = await _messageRepository.GetAllUnReadMessages(authenticatedUser.Id);

            return unReadMessages;
        }

        public async Task<IActionResult> MarkMessagesAsRead([FromBody] int[] array)
        {
            var authenticatedUser = await _userRepository.GetCurrentUser();
             
            if (array.Length == 0 )
            {
                return new OkObjectResult(new { message = "Messages marked as read successfully" });
            }

            foreach (int id in array)
            {
                Message message = await _messageRepository.FindMessageById(id);

                if (message == null)
                {
                    return new NotFoundObjectResult($"Message with ID {id} not found");
                }
                if (authenticatedUser.Id == message.receiverId)
                {
                    message.isRead = true;
                    await _messageRepository.MarkMessageAsRead(message);
                    Console.WriteLine(message);
                }
                else
                {
                    return new UnauthorizedObjectResult("Unauthorized");
                }
               
            }

            return new OkObjectResult(new { message = "Messages marked as read successfully" });
        }


        public async Task<IActionResult> GetConversationHistory(string userId, DateTime before, int count, string sort)
        {

            var authenticatedUser = await _userRepository.GetCurrentUser();

            var conversation = await _messageRepository.GetConversationHistory(userId, authenticatedUser);

            var receiver = await _messageRepository.GetReceiver(userId);


            if (receiver == null)
            {
                return new NotFoundObjectResult("User not found");
            }


            if (before != default(DateTime))
            {
                conversation = conversation.Where(m => m.timestamp < before);
            }

            if (before > DateTime.Now)
            {
                return new BadRequestObjectResult("Invalid Before Parameter");
            }

            if (sort != "asc" && sort != "desc")
            {
                return new BadRequestObjectResult("Invalid Request Parameter");
            }

            if (count <= 0)
            {
                return new BadRequestObjectResult("Invalid Request Parameter : Chat Count cannot be zero or negative");
            }

            conversation = sort == "asc" ? conversation.OrderBy(m => m.timestamp) : conversation.OrderByDescending(m => m.timestamp);

            var chat = await conversation.Select(m => new
            {
                id = m.messageId,
                senderId = m.senderId,
                receiverId = m.receiverId,
                content = m.content,
                timestamp = m.timestamp,
                isRead = m.isRead,
                isFile = m.IsFile,
                fileId = m.AttachedFile != null? m.AttachedFile.fileId : (int?)null,
                fileName = m.AttachedFile != null? m.AttachedFile.fileName : null,
                fileSize = m.AttachedFile !=null? m.AttachedFile.fileSize : (long?)null,
                caption = m.AttachedFile != null ? m.AttachedFile.caption : null,
                contentType = m.AttachedFile != null ? m.AttachedFile.contentType : null,
                filePath= m.AttachedFile != null? m.AttachedFile.filePath : null,
            }).Take(count).ToListAsync();



            if (chat.Count == 0)
            {
                //return NotFound("Conversation does not exist");
                return new OkObjectResult(new List<object>());
            }

            return new OkObjectResult(chat);


        }
    }

}

