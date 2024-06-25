using System;
using System.Collections.Generic;

namespace ThreadsProject.Core.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<PostImage> Images { get; set; } = new List<PostImage>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
        public virtual ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
        public virtual ICollection<Repost> Reposts { get; set; } = new List<Repost>();
        public virtual ICollection<UserAction> Actions { get; set; } = new List<UserAction>();
    }
}
