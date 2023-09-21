using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace RealTimeChatApi.DataAccessLayer.Models
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }
      
        public string Token { get; set; }

        [JsonIgnore]
        public virtual ICollection<Message>? sentMessages { get; set; }
        [JsonIgnore]
        public virtual ICollection<Message>? receivedMessages { get; set; }

        [JsonIgnore]
        public virtual ICollection<File>? sentFiles{ get; set; }
        [JsonIgnore]
        public virtual ICollection<File>? receivedFiles { get; set; }
    }
}
