namespace RealTimeChatApi.BusinessLogicLayer.DTOs
{
    public class LoginResponseDto
    {
        public string userId { get; set; }
        public string name { get; set; }
        public string email { get; set; }

        public string token { get; set; }
    }
}
