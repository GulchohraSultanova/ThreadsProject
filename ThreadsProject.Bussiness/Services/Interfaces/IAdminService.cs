using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.UserDtos;
using ThreadsProject.Core.Entities;
using ThreadsProject.Core.GlobalException;

namespace ThreadsProject.Bussiness.Services.Interfaces
{
    public interface IAdminService
    {
        Task<UsersGetDto> GetUserAsync(string userId);
        Task<IEnumerable<UsersGetDto>> GetAllUsersAsync(Expression<Func<User, bool>>? filter = null, params string[] includes);
        Task<IEnumerable<UsersGetDto>> SearchUsersAsync(string searchTerm);
        Task<IEnumerable<SupportDto>> GetAllSupportsAsync();
        Task DeleteSupportAsync(int supportId);
        Task<SupportDto> GetSupportRequestByIdAsync(int id);
        Task<UsersGetDto> GetUserAsyncbyUsername(string username);


        Task BanUserAsync(string userId);
        Task UnbanUserAsync(string userId);
       Task<TokenResponseDto> AdminLoginAsync(LoginDto dto);
        Task VerifyUserAsync(string userId);
        Task UnverifyUserAsync(string userId);




    }
}
