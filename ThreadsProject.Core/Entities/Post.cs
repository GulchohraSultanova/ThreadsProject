using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadsProject.Core.Entities
{
   public  class Post
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
        public virtual ICollection<PostTag> PostTags { get; set; }
        public virtual ICollection<Repost> Reposts { get; set; }
        public virtual ICollection<UserAction> Actions { get; set; }
    }
}
