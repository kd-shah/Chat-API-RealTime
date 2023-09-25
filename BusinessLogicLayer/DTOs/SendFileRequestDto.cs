namespace RealTimeChatApi.BusinessLogicLayer.DTOs
{
    public class SendFileRequestDto
    {
        public IFormFile file { get; set; }
        public string receiverId { get; set; }

        public string? caption { get; set; }
    }
}
