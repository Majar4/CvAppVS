using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace CvAppenVS.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Image {  get; set; }

        public bool IsPrivate { get; set; }


        public ICollection<UserProject> UserProjects { get; set; } = new List<UserProject>(); //Many to many till projekt
        public CV CV { get; set; } //skapar en foreign key (UserId) i CV-tabellen

        //Password och email ingår i IdentityUser
        //HasAccessTo

    }
}
