using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.FollowsDto;
using ThreadsProject.Bussiness.DTOs.PostDto;
using ThreadsProject.Bussiness.DTOs.RepostDto;


namespace ThreadsProject.Bussiness.DTOs.UserDtos
{
    public class UsersGetDto
    {
     
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool IsVerified { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string ImgUrl { get; set; }  
        public string Bio {  get; set; }
        public string IsBanned { get; set; }
        public bool IsPublic { get; set; } = true;
        public DateTime CreatedTime { get; set; }
        public ICollection<FollowDto> Followers { get; set; }
        public ICollection<FollowDto> Followings { get; set; }
  
    }
}
