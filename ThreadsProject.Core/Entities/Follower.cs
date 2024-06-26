﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadsProject.Core.Entities
{
    public  class Follower
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string FollowerUserId { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual User User { get; set; }
        public virtual User FollowerUser { get; set; }
    }
}
