using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadsProject.Bussiness.DTOs.CommentDto
{
    public  class CreateCommentDto
    {
        public int PostId { get; set; }
        public string Content { get; set; }
        public int? ParentCommentId { get; set; }
        public string? ReplyUsername { get; set; }
    }
}
