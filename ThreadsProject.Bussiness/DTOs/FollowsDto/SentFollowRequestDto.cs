using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadsProject.Bussiness.DTOs.FollowsDto
{
    public class SentFollowRequestDto
    {
        public int RequestId { get; set; }
        public string ReceiverUserId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
