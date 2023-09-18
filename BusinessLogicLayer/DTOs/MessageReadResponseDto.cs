namespace RealTimeChatApi.BusinessLogicLayer.DTOs
{
    public class MessageReadResponseDto
    {
        public int messageId { get; set; }
        public string senderId { get; set; }
        public string receiverId { get; set; }
        public string content { get; set; }
        public DateTime timestamp { get; set; }

        public Boolean isRead { get; set; }
    }
}
