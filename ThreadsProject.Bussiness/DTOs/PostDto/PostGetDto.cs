using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.CommentDto;

namespace ThreadsProject.Bussiness.DTOs.PostDto
{
    public  class PostGetDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserName { get; set; }
        public string UserImgUrl { get; set; }
        public string UserId { get; set; }
        public bool IsVerified { get; set; }
        public List<string> Images { get; set; } = new List<string>();
        public List<string> Tags { get; set; } = new List<string>();
        public List<string> Likes { get; set; } = new List<string>();


        public List<CommentGetDto> Comments { get; set; } = new List<CommentGetDto>();

    }
}
