using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadsProject.Core.Entities
{
   public  class FollowRequest
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual User Sender { get; set; }
        public virtual User Receiver { get; set; }
    
    }
}
