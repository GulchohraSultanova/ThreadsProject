using System;

namespace ThreadsProject.Bussiness.DTOs.CommentDto
{
    public class CommentLikeDto
    {
        public int Id { get; set; }
        public int CommentId { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
