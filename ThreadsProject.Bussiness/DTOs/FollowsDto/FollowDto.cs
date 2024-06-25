using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadsProject.Bussiness.DTOs.FollowsDto
{
    public class FollowDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string ImgUrl { get; set; }
        public bool IsVerified { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
