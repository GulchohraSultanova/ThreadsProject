using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.FollowsDto;


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
        public bool IsPublic { get; set; } = true;
        public List<FollowDto> Followers { get; set; } = new List<FollowDto>();
        public List<FollowDto> Followings { get; set; } = new List<FollowDto>();
    }
}
