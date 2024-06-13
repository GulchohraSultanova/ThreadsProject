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
        public string ?  ImgUrl { get; set; } = "https://static.vecteezy.com/system/resources/previews/002/534/006/original/social-media-chatting-online-blank-profile-picture-head-and-body-icon-people-standing-icon-grey-background-free-vector.jpg";
        public string Bio {  get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsPublic { get; set; } = true;
        public bool IsVerified { get; set; }
        public bool IsDeleted   { get; set; }
     
        public DateTime? BanStartDate { get; set; }
        public DateTime? BanEndDate { get; set; }
        public int BanCount { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Follower> Followers { get; set; }
        public virtual ICollection<Following> Following { get; set; }
        public virtual ICollection<UserAction> Actions { get; set; }
        public virtual ICollection<Repost> Reposts { get; set; }
        public virtual ICollection<Request> SentRequests { get; set; }
        public virtual ICollection<Request> ReceivedRequests { get; set; }
    }

}

