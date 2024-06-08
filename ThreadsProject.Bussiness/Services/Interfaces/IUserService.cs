using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.UserDtos;
using ThreadsProject.Core.Entities;

namespace ThreadsProject.Bussiness.Services.Interfaces
{
   public  interface IUserService
    {
        Task Register(RegisterDto registerDto);
        Task<IEnumerable<UsersGetDto>> GetAllUsersAsync(Expression<Func<User, bool>>? filter = null, params string[] includes);
    }
}

