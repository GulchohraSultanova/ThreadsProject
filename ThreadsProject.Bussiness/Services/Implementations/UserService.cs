using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.UserDtos;
using ThreadsProject.Bussiness.ExternalServices.Interfaces;
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
                if (user == null)
                {
                    throw new GlobalAppException("User not found");
                }

                var userDto = _mapper.Map<UsersGetDto>(user);
                userDto.ImgUrl = user.ImgUrl;

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
                var user = await _userManager.FindByNameAsync(dto.UserNameOrEmail);
                if (user == null)
                {
                    user = await _userManager.FindByEmailAsync(dto.UserNameOrEmail);
                    if (user == null)
                    {
                        throw new GlobalAppException("User not found with the provided username or email.");
                    }
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

    }
}
