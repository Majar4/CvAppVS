using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace CvAppen.Data
{
    public class User : IdentityUser
    {

        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? Image {  get; set; }
        public bool IsPrivate { get; set; }

        public bool IsActive { get; set; }


        public ICollection<Message> RecievedMessages { get; set; } = new List<Message>();

        public ICollection<Message> SentMessages {  get; set; } = new List<Message>();  

        public ICollection<UserProject> UserProjects { get; set; } = new List<UserProject>(); 
        public CV CV { get; set; } 

    }
}
