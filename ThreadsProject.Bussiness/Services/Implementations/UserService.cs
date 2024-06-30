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
using ThreadsProject.Bussiness.DTOs.FollowsDto;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;

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
        private readonly Cloudinary _cloudinary;

        public UserService(IMapper mapper, UserManager<User> userManager, ILogger<UserService> logger, ThreadsContext context, ITokenService tokenService, IUserEmailSender emailSender, IConfiguration configuration, Cloudinary cloudinary)
        {
            _mapper = mapper;
            _userManager = userManager;
            _logger = logger;
            _context = context;
            _tokenService = tokenService;
            _emailSender = emailSender;
            _configuration = configuration;
            _cloudinary = cloudinary;
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
            IQueryable<User> query = _context.Users
                .Include(u => u.Followers)
                    .ThenInclude(f => f.FollowerUser)
                .Include(u => u.Following)
                    .ThenInclude(f => f.FollowingUser)
                .Where(u => !u.IsDeleted && u.EmailConfirmed && !u.IsBanned && !u.AdminOrUser);

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



        public async Task<UsersGetDto> GetUserByIdAsync(string userId)
        {
            try
            {
                var user = await _userManager.Users
                    .Include(u => u.Followers)
                        .ThenInclude(f => f.FollowerUser)
                    .Include(u => u.Following)
                        .ThenInclude(f => f.FollowingUser)
                    .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted && u.EmailConfirmed && !u.IsBanned && !u.AdminOrUser);

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
   



        public async Task<TokenResponseDto> LoginAsync(LoginDto dto)
        {
            try
            {
                var user = await _userManager.Users
                    .Where(u => (u.UserName == dto.UserNameOrEmail || u.Email == dto.UserNameOrEmail) && !u.IsDeleted  && !u.AdminOrUser)
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    throw new GlobalAppException("User not found with the provided username or email.");
                }

                if (!user.EmailConfirmed)
                {
                    throw new GlobalAppException("Email not verified. Please confirm your email.");
                }
                if (user.IsBanned)
                {
                    throw new GlobalAppException("User is banned!");
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

                if (!string.IsNullOrEmpty(userEditDto.ImgUrl))
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(userEditDto.ImgUrl),
                        UseFilename = true,
                        UniqueFilename = false,
                        Overwrite = true
                    };
                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        user.ImgUrl = uploadResult.SecureUrl.AbsoluteUri;
                    }
                    else
                    {
                        throw new GlobalAppException("Image upload failed.");
                    }
                }

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
        public async Task<UsersGetDto> GetUserByUsernameAsync(string username, string requesterId)
        {
            try
            {
                var user = await _userManager.Users
                    .Include(u => u.Followers)
                        .ThenInclude(f => f.FollowerUser)
                    .Include(u => u.Following)
                        .ThenInclude(f => f.FollowingUser)
                    .FirstOrDefaultAsync(u => u.UserName == username && !u.IsDeleted && !u.IsBanned && u.EmailConfirmed && !u.AdminOrUser);

                if (user == null)
                {
                    throw new GlobalAppException("User not found.");
                }

                var isFollowing = await _context.Followers
                    .AnyAsync(f => f.UserId == user.Id && f.FollowerUserId == requesterId);

                var userDto = _mapper.Map<UsersGetDto>(user);

              
                return userDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the user by username");
                throw new GlobalAppException("User not found!", ex);
            }
        }
        public async Task<UsersGetDto> GetUserByIdAsync(string id, string requesterId)
        {
            try
            {
                var user = await _userManager.Users
                    .Include(u => u.Followers)
                        .ThenInclude(f => f.FollowerUser)
                    .Include(u => u.Following)
                        .ThenInclude(f => f.FollowingUser)
                    .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted && !u.IsBanned && u.EmailConfirmed && !u.AdminOrUser);

                if (user == null)
                {
                    throw new GlobalAppException("User not found.");
                }

                var isFollowing = await _context.Followers
                    .AnyAsync(f => f.UserId == user.Id && f.FollowerUserId == requesterId);

                var userDto = _mapper.Map<UsersGetDto>(user);


                return userDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the user by username");
                throw new GlobalAppException("User not found!", ex);
            }
        }
        public async Task<IEnumerable<UsersGetDto>> GetRandomUsersAsync(int count = 10, string currentUserId = null)
        {
            try
            {
                var random = new Random();
                var users = await _userManager.Users
                    .Where(u => !u.IsDeleted && u.IsPublic && !u.IsBanned && u.EmailConfirmed && u.Id != currentUserId && !u.AdminOrUser)
                  .Include(u => u.Followers)
                .ThenInclude(f => f.FollowerUser)
                .Include(u => u.Following)
                .ThenInclude(f => f.FollowingUser)
                    .ToListAsync(); // Veritabanı sorgusunu burada tamamlayın

                var randomUsers = users
                    .OrderBy(u => random.Next())
                    .Take(count)
                    .ToList(); // Sonuçları bellek içi sıralayıp listeleyin

                return randomUsers.Select(user => _mapper.Map<UsersGetDto>(user));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetRandomUsersAsync: {ex.Message}");
                throw new Exception("An error occurred while retrieving random users", ex);
            }
        }






        public async Task<IEnumerable<UsersGetDto>> SearchUsersAsync(string searchTerm)
        {
            searchTerm = searchTerm.Trim();
            if (string.IsNullOrEmpty(searchTerm))
            {
                throw new ArgumentException("Search term cannot be empty", nameof(searchTerm));
            }

            var users = await _userManager.Users
                .Where(u => !u.IsDeleted && u.EmailConfirmed && !u.IsBanned && !u.AdminOrUser && u.UserName.StartsWith(searchTerm))
                .Include(u => u.Followers)
                .ThenInclude(f => f.FollowerUser)
                .Include(u => u.Following)
                .ThenInclude(f => f.FollowingUser)
                .ToListAsync();

            return users.Select(user => _mapper.Map<UsersGetDto>(user));
        }

   
     public async Task SendPasswordResetLinkAsync(ForgotPasswordDto forgot)
        {
            var user = await _userManager.FindByEmailAsync(forgot.Email);
            if (user == null || user.IsDeleted)
            {
                throw new GlobalAppException("User not found with the provided email.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"http://frontend-final-navy.vercel.app/reset-password?token={Uri.EscapeDataString(token)}&userId={Uri.EscapeDataString(user.Id)}";

            await _emailSender.SendEmailAsync(user.Email, "Reset your password", $"Please reset your password by clicking this link: <a href='{resetLink}'>here</a>.");
        }


        public async Task ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmPassword)
            {
                throw new GlobalAppException("Passwords do not match.");
            }

            try
            {
                var user = await _userManager.FindByIdAsync(resetPasswordDto.UserId);
                if (user == null)
                {
                    throw new GlobalAppException("User not found.");
                }

                var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
                if (!resetPassResult.Succeeded)
                {
                    var errors = string.Join(", ", resetPassResult.Errors.Select(e => e.Description));
                    _logger.LogError($"Password reset failed: {errors}");
                    throw new GlobalAppException($"Password reset failed: {errors}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while resetting the password.");
                throw new GlobalAppException("An error occurred while resetting the password.", ex);
            }
        }
        public async Task ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new GlobalAppException("User ID cannot be null or empty");
            }

            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmPassword)
            {
                throw new GlobalAppException("Passwords do not match.");
            }

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new GlobalAppException("User not found.");
                }

                var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, changePasswordDto.CurrentPassword);
                if (!isCurrentPasswordValid)
                {
                    throw new GlobalAppException("Current password is incorrect.");
                }

                if (changePasswordDto.NewPassword == changePasswordDto.CurrentPassword)
                {
                    throw new GlobalAppException("New password cannot be the same as the current password.");
                }

                var resetPassResult = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
                if (!resetPassResult.Succeeded)
                {
                    var errors = string.Join(", ", resetPassResult.Errors.Select(e => e.Description));
                    _logger.LogError($"Password change failed: {errors}");
                    throw new GlobalAppException($"Password change failed: {errors}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while changing the password.");
                throw new GlobalAppException("An error occurred while changing the password.", ex);
            }
        }











    }
}
