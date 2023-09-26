namespace RealTimeChatApi.BusinessLogicLayer.DTOs
{
    public class SendFileRequestDto
    {
        public List<IFormFile> files { get; set; }
        public string receiverId { get; set; }

        public string? caption { get; set; }
    }
}
