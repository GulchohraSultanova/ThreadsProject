using System;
using System.Collections.Generic;
using ThreadsProject.Bussiness.DTOs.PostDto;

namespace ThreadsProject.Bussiness.DTOs.CommentDto
{
    public class CommentWithPostDto
    {
        public int CommentId { get; set; }
        public string CommentContent { get; set; }
        public DateTime CommentCreatedDate { get; set; }
        public PostGetDto Post { get; set; }
    }
}
