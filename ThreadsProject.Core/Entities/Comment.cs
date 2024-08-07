﻿using System;
using System.Collections.Generic;

namespace ThreadsProject.Core.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ParentCommentId { get; set; } // Add this line
        public string ? ReplyUsername { get; set; }
        public virtual Comment ParentComment { get; set; } // Add this line
        public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();

        public virtual Post Post { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<CommentLike> CommentLikes { get; set; } = new List<CommentLike>();
    }
}
