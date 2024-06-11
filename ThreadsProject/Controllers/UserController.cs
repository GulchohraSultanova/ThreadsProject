using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.UserDtos;
using ThreadsProject.Bussiness.GlobalException;
using ThreadsProject.Bussiness.Services.Interfaces;
using ThreadsProject.Core.Entities;

namespace ThreadsProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly UserManager<User> _userManager;

        public UserController(IUserService userService, ILogger<UserController> logger, UserManager<User> userManager)
        {
            _userService = userService;
            _logger = logger;
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Error = "Invalid input data"
                });
            }

            try
            {
                await _userService.Register(registerDto);
                return StatusCode(StatusCodes.Status201Created, new
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = "User registered successfully. Please check your email to confirm your account."
                });
            }
            catch (GlobalAppException ex)
            {
                _logger.LogError(ex, "An error occurred while registering the user");
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Error = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpGet("confirmemail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string token, [FromQuery] string userId)
        {
            _logger.LogInformation($"ConfirmEmail called with token: {token} and userId: {userId}");

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Invalid token or user ID.");
                return BadRequest("Invalid token or user ID.");
            }

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning($"User not found with userId: {userId}");
                    return BadRequest("Invalid token or user ID.");
                }

                // Confirm the email
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"Email confirmed successfully for userId: {userId}");
                    return Ok("Email confirmed successfully.");
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning($"Email confirmation failed for userId: {userId}, errors: {errors}");
                    return BadRequest($"Email confirmation failed: {errors}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while confirming the email");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }





        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return StatusCode(StatusCodes.Status200OK, new
            {
                StatusCode = StatusCodes.Status200OK,
                Data = users
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _userService.LoginAsync(loginDto);

                return Ok(result);
            }
            catch (GlobalAppException ex)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Error = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while logging in the user");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred while logging in the user" });
            }
        }

        [HttpGet("userdata")]
        [Authorize]
        public async Task<IActionResult> GetUserData()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new { message = "Invalid token" });
            }

            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Data = user
                });
            }
            catch (GlobalAppException ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the user");
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Error = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "An unexpected error occurred. Please try again later."
                });
            }
        }
    }
}
