using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadsProject.Core.Entities
{
    public  class PostImage
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string ImageUrl { get; set; }
        public virtual Post Post { get; set; }
    }
}
