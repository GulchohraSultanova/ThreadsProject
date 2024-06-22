using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using ThreadsProject.Core.Entities;
using ThreadsProject.Bussiness.DTOs.UserDtos;
using ThreadsProject.Core.GlobalException;
using Microsoft.Extensions.Logging;
using AutoMapper;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;
using System.Linq;
using ThreadsProject.Bussiness.ExternalServices.Interfaces;
using ThreadsProject.Bussiness.Services.Interfaces;
using ThreadsProject.Data.DAL;

namespace ThreadsProject.Bussiness.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly ThreadsContext _context;
        private readonly ITokenService _tokenService;
        private readonly IUserEmailSender _emailSender;
        private readonly IConfiguration _configuration;

        public UserService(IMapper mapper, UserManager<User> userManager, ILogger<UserService> logger, ThreadsContext context, ITokenService tokenService, IUserEmailSender emailSender, IConfiguration configuration)
        {
            _mapper = mapper;
            _userManager = userManager;
            _logger = logger;
            _context = context;
            _tokenService = tokenService;
            _emailSender = emailSender;
            _configuration = configuration;
        }

        public async Task Register(RegisterDto registerDto)
        {
            try
            {
                var user = _mapper.Map<User>(registerDto);
                user.Name = registerDto.FirstName;
                user.Surname = registerDto.LastName;
                user.ImgUrl = "https://static.vecteezy.com/system/resources/previews/002/534/006/original/social-media-chatting-online-blank-profile-picture-head-and-body-icon-people-standing-icon-grey-background-free-vector.jpg"; // Default profile picture
                user.Bio = "";

                var existingUser = await _userManager.Users
                    .Where(u => u.UserName == user.UserName && !u.IsDeleted)
                    .FirstOrDefaultAsync();
                if (existingUser != null)
                {
                    throw new GlobalAppException("Username is already taken.");
                }

                var existingEmail = await _userManager.Users
                    .Where(u => u.Email == user.Email && !u.IsDeleted)
                    .FirstOrDefaultAsync();
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

                // Generate email confirmation token
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                // Get the client URL from app settings
                var clientUrl = _configuration["AppSettings:ClientURL"];
                var confirmationLink = $"{clientUrl}/api/user/confirmemail?token={Uri.EscapeDataString(token)}&userId={Uri.EscapeDataString(user.Id)}";

                // Send confirmation email
                await _emailSender.SendEmailAsync(user.Email, "Confirm your email", $"Please confirm your email by clicking this link: <a href='{confirmationLink}'>here</a>.");
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
            IQueryable<User> query = _context.Users.Where(u => !u.IsDeleted);

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
                return userDto;
            });
        }


        public async Task<UsersGetDto> GetUserByIdAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null || user.IsDeleted)
                {
                    throw new GlobalAppException("User not found");
                }

                var userDto = _mapper.Map<UsersGetDto>(user);
                userDto.ImgUrl = user.ImgUrl;
                userDto.Bio = user.Bio;
                userDto.IsPublic = user.IsPublic;

                return userDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the user");
                throw new GlobalAppException("An unexpected error occurred while retrieving the user", ex);
            }
        }

        public async Task<TokenResponseDto> LoginAsync(LoginDto dto)
        {
            try
            {
                var user = await _userManager.Users
                    .Where(u => (u.UserName == dto.UserNameOrEmail || u.Email == dto.UserNameOrEmail) && !u.IsDeleted)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    throw new GlobalAppException("User not found with the provided username or email.");
                }

                if (!user.EmailConfirmed)
                {
                    throw new GlobalAppException("Email not verified. Please confirm your email.");
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
                _logger.LogError(ex, "An error occurred while logging in the user");
                throw new GlobalAppException("An unexpected error occurred while logging in the user", ex);
            }
        }


        public async Task EditUserAsync(string userId, UserEditDto userEditDto)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new GlobalAppException("User ID cannot be null or empty");
            }

            try
            {
                // Username boşluk içeriyorsa hata fırlat
                if (userEditDto.UserName.Contains(" "))
                {
                    throw new GlobalAppException("Username cannot contain spaces.");
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new GlobalAppException("User not found");
                }

                if (!string.Equals(user.UserName, userEditDto.UserName, StringComparison.OrdinalIgnoreCase))
                {
                    var existingUser = await _userManager.Users
                        .Where(u => u.UserName == userEditDto.UserName && !u.IsDeleted)
                        .FirstOrDefaultAsync();
                    if (existingUser != null && existingUser.Id != user.Id)
                    {
                        throw new GlobalAppException("Username is already taken.");
                    }
                }

                user.UserName = userEditDto.UserName;
                user.Bio = userEditDto.Bio;
                user.IsPublic = userEditDto.IsPublic;
                user.ImgUrl = userEditDto.ImgUrl;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new GlobalAppException($"User update failed: {errors}");
                }
            }
            catch (GlobalAppException ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the user");
                throw new GlobalAppException("An unexpected error occurred while updating the user", ex);
            }
        }

        public async Task DeleteUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new GlobalAppException("User ID cannot be null or empty");
            }

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new GlobalAppException("User not found");
                }

                user.IsDeleted = true;
                string guid = $"_deleted_{Guid.NewGuid()}";

                if (user.UserName.Length + guid.Length > 256)  // Uzunluk kontrolü
                {
                    user.UserName = user.UserName.Substring(0, 256 - guid.Length);
                }
                user.UserName += guid;

                if (user.Email.Length + guid.Length > 512)  // Uzunluk kontrolü
                {
                    user.Email = user.Email.Substring(0, 512 - guid.Length);
                }
                user.Email += guid;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new GlobalAppException($"User deletion failed: {errors}");
                }
            }
            catch (GlobalAppException ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the user");
                throw new GlobalAppException("An unexpected error occurred while deleting the user", ex);
            }
        }
        public async Task<UsersGetDto> GetUserByUsernameAsync(string username)
        {
            try
            {
                var user = await _userManager.Users
                    .Where(u => u.UserName == username && !u.IsDeleted)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    throw new GlobalAppException("User not found.");
                }

                var userDto = _mapper.Map<UsersGetDto>(user);
                userDto.ImgUrl = user.ImgUrl;
                userDto.Bio = user.Bio;
                userDto.IsPublic = user.IsPublic;

                return userDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the user by username");
                throw new GlobalAppException("User not found!", ex);
            }
        }


    }
}
