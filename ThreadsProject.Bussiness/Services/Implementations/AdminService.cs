using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.FollowsDto;
using ThreadsProject.Bussiness.DTOs.UserDtos;
using ThreadsProject.Bussiness.ExternalServices.Interfaces;
using ThreadsProject.Bussiness.Services.Interfaces;
using ThreadsProject.Core.Entities;
using ThreadsProject.Core.GlobalException;
using ThreadsProject.Core.RepositoryAbstracts;
using ThreadsProject.Data.DAL;

namespace ThreadsProject.Bussiness.Services.Implementations
{
    public class AdminService:IAdminService
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminService> _logger;
        private readonly ThreadsContext _context;
        private readonly ISupportRepository _supportRepository;
        private readonly ITokenService _tokenService;

        public AdminService(UserManager<User> userManager, IMapper mapper, ThreadsContext context, ISupportRepository supportRepository, ILogger<AdminService> logger, ITokenService tokenService)
        {
            _userManager = userManager;
            _mapper = mapper;

            _context = context;
            _supportRepository = supportRepository;
            _logger = logger;
            _tokenService = tokenService;
        }
        public async Task<TokenResponseDto> AdminLoginAsync(LoginDto dto)
        {
            try
            {
                var user = await _userManager.Users
                    .Where(u => (u.UserName == dto.UserNameOrEmail || u.Email == dto.UserNameOrEmail))
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    throw new GlobalAppException("Admin not found with the provided username or email.");
                }

                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                if (!isAdmin)
                {
                    throw new GlobalAppException("User is not an admin.");
                }


           
                var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
                if (!isPasswordValid)
                {
                    throw new GlobalAppException("Invalid password.");
                }

                var tokenResponse = _tokenService.CreateToken(user);
                return tokenResponse;
            }
            catch (GlobalAppException ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while logging in the admin");
                throw new GlobalAppException("An unexpected error occurred while logging in the admin", ex);
            }
        }


        public async Task<UsersGetDto> GetUserAsync(string userId)
        {
            try
            {
                var user = await _userManager.Users.Where(u => !u.AdminOrUser)
                    .Include(u => u.Followers)
                        .ThenInclude(f => f.FollowerUser)
                    .Include(u => u.Following)
                        .ThenInclude(f => f.FollowingUser)
                    .FirstOrDefaultAsync(u => u.Id == userId );

                if (user == null)
                {
                    throw new GlobalAppException("User not found");
                }

                var userDto = _mapper.Map<UsersGetDto>(user);
                return userDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the user");
                throw new GlobalAppException("An unexpected error occurred while retrieving the user", ex);
            }
        }
        public async Task<UsersGetDto> GetUserAsyncbyUsername(string username)
        {
            try
            {
                var user = await _userManager.Users.Where(u => !u.AdminOrUser)
                    .Include(u => u.Followers)
                        .ThenInclude(f => f.FollowerUser)
                    .Include(u => u.Following)
                        .ThenInclude(f => f.FollowingUser)
                    .FirstOrDefaultAsync(u => u.UserName == username);

                if (user == null)
                {
                    throw new GlobalAppException("User not found");
                }

                var userDto = _mapper.Map<UsersGetDto>(user);
                return userDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the user");
                throw new GlobalAppException("An unexpected error occurred while retrieving the user", ex);
            }
        }
        public async Task<IEnumerable<UsersGetDto>> GetAllUsersAsync(Expression<Func<User, bool>>? filter = null, params string[] includes)
        {
            IQueryable<User> query = _context.Users.Where(u=> !u.AdminOrUser)
                .Include(u => u.Followers)
                    .ThenInclude(f => f.FollowerUser)
                .Include(u => u.Following)
                    .ThenInclude(f => f.FollowingUser);
               

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            var users = await query.ToListAsync();
            return users.Select(user =>
            {
                var userDto = _mapper.Map<UsersGetDto>(user);
                userDto.ImgUrl = user.ImgUrl;
                userDto.Followers = _mapper.Map<List<FollowDto>>(user.Followers);
                userDto.Followings = _mapper.Map<List<FollowDto>>(user.Following);
                return userDto;
            });
        }
        public async Task<IEnumerable<UsersGetDto>> SearchUsersAsync(string searchTerm)
        {
            searchTerm = searchTerm.Trim();
            if (string.IsNullOrEmpty(searchTerm))
            {
                throw new ArgumentException("Search term cannot be empty", nameof(searchTerm));
            }

            var users = await _userManager.Users
                .Where(u => u.UserName.StartsWith(searchTerm)).Where(u => !u.AdminOrUser)
                .Include(u => u.Followers)
                .ThenInclude(f => f.FollowerUser)
                .Include(u => u.Following)
                .ThenInclude(f => f.FollowingUser)
                .ToListAsync();

            return users.Select(user => _mapper.Map<UsersGetDto>(user));
        }
        public async Task<IEnumerable<SupportDto>> GetAllSupportsAsync()
        {
            var supports = await _supportRepository.GetAllAsync(null, "User");
            return _mapper.Map<IEnumerable<SupportDto>>(supports);
        }

        public async Task DeleteSupportAsync(int supportId)
        {
            var support = await _supportRepository.GetByIdAsync(supportId);
            if (support == null)
            {
                throw new GlobalAppException("Support request not found.");
            }

            try
            {
                await _supportRepository.DeleteAsync(support);
            }
            catch (Exception ex)
            {
                throw new GlobalAppException("An error occurred while deleting the support request.", ex);
            }
        }
        public async Task<SupportDto> GetSupportRequestByIdAsync(int id)
        {
            var supportRequest = await _supportRepository.GetByIdAsync(id);
            if (supportRequest == null)
            {
                throw new GlobalAppException("Support request not found");
            }
            return _mapper.Map<SupportDto>(supportRequest);
        }
        public async Task BanUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new GlobalAppException("User not found");
            }

            user.IsBanned = true;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new GlobalAppException("Failed to update user");
            }
        }

        public async Task UnbanUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new GlobalAppException("User not found");
            }

            user.IsBanned = false;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new GlobalAppException("Failed to update user");
            }
        }

        public async Task VerifyUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new GlobalAppException("User not found");
            }

            user.IsVerified = true;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new GlobalAppException("Failed to update user");
            }
        }

        public async Task UnverifyUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new GlobalAppException("User not found");
            }

            user.IsVerified = false;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new GlobalAppException("Failed to update user");
            }
        }
    }
}
