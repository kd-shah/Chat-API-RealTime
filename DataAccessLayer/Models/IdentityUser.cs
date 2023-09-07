using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace RealTimeChatApi.DataAccessLayer.Models
{
    public class IdentityUSer : IdentityUser
    {
        
        //public int userId { get; set; }
        public string Name { get; set; }
        //public string email { get; set; }

        //public string password { get; set; }
        public string Token { get; set; }

        //public virtual ICollection<Message>? sentMessages { get; set; }
        //public virtual ICollection<Message>? receivedMessages { get; set; }
    }
}
