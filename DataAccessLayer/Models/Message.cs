using System.ComponentModel.DataAnnotations.Schema;

namespace RealTimeChatApi.DataAccessLayer.Models
{
    public class Message
    {
        public int messageId { get; set; }
        public int senderId { get; set; }
        public int receiverId { get; set; }
        public string content { get; set; }
        public DateTime timestamp { get; set; }

        [ForeignKey("senderId")]
        public virtual AppUser sender { get; set; }
        [ForeignKey("receiverId")]
        public virtual AppUser receiver { get; set; }
    }
}
