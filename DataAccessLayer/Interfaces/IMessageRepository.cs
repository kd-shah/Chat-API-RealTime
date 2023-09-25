using Microsoft.AspNetCore.Mvc;
using RealTimeChatApi.BusinessLogicLayer.DTOs;
using RealTimeChatApi.DataAccessLayer.Models;

namespace RealTimeChatApi.DataAccessLayer.Interfaces
{
    public interface IMessageRepository
    {
        Task<AppUser> GetReceiver(string receiverId);

        Task<Message> GetMessage(int messageId);

        Task<IActionResult> SendMessage(Message message);

        Task<IActionResult> EditMessage();

        Task<IActionResult> DeleteMessage(Message message);

        Task<IQueryable<Message>> GetConversationHistory(string id, AppUser authenticateduser);

        Task<IQueryable<Message>> SearchMessages(string userId, string query);

        Task<Message> FindMessageById(int messageId);

        Task<IActionResult> GetAllUnReadMessages(string authenticatedUserId);

        Task<IActionResult> MarkMessageAsRead(Message message);

        Task<IActionResult> SaveMessageChanges();
    }
}
