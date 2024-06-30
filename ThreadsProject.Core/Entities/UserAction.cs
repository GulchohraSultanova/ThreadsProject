using Azure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadsProject.Core.Entities
{
   public class UserAction
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int? PostId { get; set; }
        public int? CommentId { get; set; }
        public string TargetUserId { get; set; }

        public string ActionType { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual User User { get; set; }
        public virtual Post Post { get; set; }
        public virtual Comment Comment { get; set; }

    }
}
