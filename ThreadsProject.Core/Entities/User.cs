using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadsProject.Core.Entities
{
    public  class User:IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string ImgUrl { get; set; } = "https://static.vecteezy.com/system/resources/previews/002/534/006/original/social-media-chatting-online-blank-profile-picture-head-and-body-icon-people-standing-icon-grey-background-free-vector.jpg";
    }

}

