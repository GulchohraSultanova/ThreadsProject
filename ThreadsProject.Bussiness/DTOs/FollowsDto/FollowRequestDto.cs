using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadsProject.Bussiness.DTOs.FollowsDto
{
    public  class FollowRequestDto
    {
        public int RequestId { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string SenderUserName { get; set; }
        public string SenderSurname { get; set; }
        public string ImgUrl { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
