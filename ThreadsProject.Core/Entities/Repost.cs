using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadsProject.Core.Entities
{
    public class Repost
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual Post Post { get; set; }
        public virtual User User { get; set; }
    }
}
