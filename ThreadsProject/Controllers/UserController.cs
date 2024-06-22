using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.UserDtos;
using ThreadsProject.Core.GlobalException;
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
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Error = "Invalid token or user ID."
                });
            }

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning($"User not found with userId: {userId}");
                    return BadRequest(new
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Error = "Invalid token or user ID."
                    });
                }

                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"Email confirmed successfully for userId: {userId}");
                    return Ok(new
                    {
                        StatusCode = StatusCodes.Status200OK,
                        Message = "Email confirmed successfully."
                    });
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning($"Email confirmation failed for userId: {userId}, errors: {errors}");
                    return BadRequest(new
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Error = $"Email confirmation failed: {errors}"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while confirming the email");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return StatusCode(StatusCodes.Status200OK, new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Data = users
                });
            }
            catch (GlobalAppException ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving users");
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

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
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
                var result = await _userService.LoginAsync(loginDto);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Data = result
                });
            }
            catch (GlobalAppException ex)
            {
                _logger.LogError(ex, "An error occurred while logging in the user");
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Error = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while logging in the user");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpGet("userdata")]
        [Authorize]
        public async Task<IActionResult> GetUserData()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Error = "Invalid token"
                });
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

        [HttpPut("edit")]
        [Authorize]
        public async Task<IActionResult> EditUser([FromBody] UserEditDto userEditDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Error = "Invalid token"
                });
            }

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
                await _userService.EditUserAsync(userId, userEditDto);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "User updated successfully."
                });
            }
            catch (GlobalAppException ex)
            {
                _logger.LogError(ex, "An error occurred while updating the user");
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
        [HttpDelete("delete")]
        [Authorize]
        public async Task<IActionResult> DeleteUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Error = "Invalid token"
                });
            }

            try
            {
                await _userService.DeleteUserAsync(userId);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "User deleted successfully."
                });
            }
            catch (GlobalAppException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the user");
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
        [HttpGet("username")]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            try
            {
                var user = await _userService.GetUserByUsernameAsync(username);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Data = user
                });
            }
            catch (GlobalAppException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Error = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving the user by username");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "An unexpected error occurred. Please try again later."
                });
            }
        }


    }
}
