﻿using System;
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
        Task<TokenResponseDto> LoginAsync(LoginDto dto);
        Task<UsersGetDto> GetUserByIdAsync(string userId);
        Task EditUserAsync(string userId, UserEditDto userEditDto);
        Task DeleteUserAsync(string userId);
        Task<UsersGetDto> GetUserByUsernameAsync(string username, string requesterId);
        Task<IEnumerable<UsersGetDto>> GetRandomUsersAsync(int count = 10, string currentUserId = null);
        Task<IEnumerable<UsersGetDto>> SearchUsersAsync(string searchTerm);
        Task SendPasswordResetLinkAsync(ForgotPasswordDto forgot);
        Task ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<UsersGetDto> GetUserByIdAsync(string id, string requesterId);
        Task ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto);
        Task SendUrlToUserAsync(SendUrlDto sendUrlDto);




    }
}


