using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.UserDtos;
using ThreadsProject.Core.Entities;

namespace ThreadsProject.Bussiness.ExternalServices.Interfaces
{
  public  interface  ITokenService
    {
        TokenResponseDto CreateToken(User user,int expireDate=60);
    }
}
