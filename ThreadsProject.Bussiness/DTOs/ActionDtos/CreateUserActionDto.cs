using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadsProject.Bussiness.DTOs.ActionDtos
{
    public  class CreateUserActionDto
    {
        public string UserId { get; set; }
        public int? PostId { get; set; }
        public int? CommentId { get; set; }
        public string ActionType { get; set; }
        public string TargetUserId { get; set; }
    }
}
