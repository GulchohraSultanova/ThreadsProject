using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadsProject.Bussiness.DTOs.UserDtos
{
    public class UsersGetDto
    {
     
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string ImgUrl { get; set; }  
        public string Bio {  get; set; }
        public bool IsPublic { get; set; } = true;
    }
}
