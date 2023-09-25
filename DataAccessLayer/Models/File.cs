using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RealTimeChatApi.DataAccessLayer.Models
{
    public class File
    {
        [Key]
        public int fileId { get; set; }
        public string fileName { get; set; }
        public long fileSize { get; set; }

        public string? caption { get; set; }
        public string contentType { get; set; }
        public DateTime uploadDateTime { get; set; }
        public string senderId { get; set; }
        public string receiverId { get; set; }
        public string filePath { get; set; }
        public bool isRead { get; set; }

        [ForeignKey("senderId")]
        [JsonIgnore]
        public virtual AppUser sender { get; set; }
        [ForeignKey("receiverId")]
        [JsonIgnore]
        public virtual AppUser receiver { get; set; }
        public int messageId { get; set; }

        [JsonIgnore]
        public virtual Message Message { get; set; }


    }
}
