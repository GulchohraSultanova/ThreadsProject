using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.UserDtos;
using ThreadsProject.Bussiness.GlobalException;
using ThreadsProject.Bussiness.Services.Interfaces;
using ThreadsProject.Core.Entities;
using ThreadsProject.Data.DAL;

namespace ThreadsProject.Bussiness.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly ThreadsContext _context;

        public UserService(IMapper mapper, UserManager<User> userManager, ILogger<UserService> logger, ThreadsContext context)
        {
            _mapper = mapper;
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }

        public async Task Register(RegisterDto registerDto)
        {
            try
            {
                var user = _mapper.Map<User>(registerDto);

                user.Name = registerDto.FirstName;
                user.Surname = registerDto.LastName;

                var existingUser = await _userManager.FindByNameAsync(user.UserName);
                if (existingUser != null)
                {
                    throw new GlobalAppException("Username is already taken.");
                }

                var existingEmail = await _userManager.FindByEmailAsync(user.Email);
                if (existingEmail != null)
                {
                    throw new GlobalAppException("Email is already in use.");
                }

                var result = await _userManager.CreateAsync(user, registerDto.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new GlobalAppException($"User creation failed: {errors}");
                }
            }
            catch (GlobalAppException ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering the user");
                throw new GlobalAppException("An unexpected error occurred while registering the user", ex);
            }
        }

        public async Task<IEnumerable<UsersGetDto>> GetAllUsersAsync(Expression<Func<User, bool>>? filter = null, params string[] includes)
        {
            IQueryable<User> query = _context.Users;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            var users = await query.ToListAsync();
            return users.Select(user => _mapper.Map<UsersGetDto>(user));
        }
    }
}
