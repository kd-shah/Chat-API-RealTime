using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RealTimeChatApi.DataAccessLayer.Models
{
    public class Message
    {
        [Key]
        public int messageId { get; set; }
        public string senderId { get; set; }
        public string receiverId { get; set; }
        public string content { get; set; }
        public DateTime timestamp { get; set; }
        
        public bool isRead { get; set; } 

        [ForeignKey("senderId")]
        [JsonIgnore]
        public virtual AppUser sender { get; set; }
        [ForeignKey("receiverId")]
        [JsonIgnore]
        public virtual AppUser receiver { get; set; }

        public bool IsFile { get; set; }
        
        public int? fileId { get; set; }
        [JsonIgnore]
        public virtual File AttachedFile { get; set; } 
    }
}

