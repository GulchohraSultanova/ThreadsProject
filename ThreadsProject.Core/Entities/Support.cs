using System;

namespace ThreadsProject.Core.Entities
{
    public class Support
    {
        public int Id { get; set; }
        public string UserId { get; set; }
     
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
      

        public virtual User User { get; set; }
       
    }
}
