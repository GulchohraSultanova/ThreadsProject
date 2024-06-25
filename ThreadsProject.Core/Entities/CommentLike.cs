using System;

namespace ThreadsProject.Core.Entities
{
    public class CommentLike
    {
        public int Id { get; set; }
        public int CommentId { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual Comment Comment { get; set; }
        public virtual User User { get; set; }
    }
}
