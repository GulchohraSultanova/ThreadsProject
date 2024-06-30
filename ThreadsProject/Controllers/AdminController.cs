using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThreadsProject.Bussiness.DTOs.TagDto;
using ThreadsProject.Bussiness.DTOs.UserDtos;
using ThreadsProject.Bussiness.Services.Interfaces;
using ThreadsProject.Core.GlobalException;

namespace ThreadsProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ISupportService _supportService;
        private readonly ITagService _tagService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ISupportService supportService, IAdminService adminService, ILogger<AdminController> logger, ITagService tagService)
        {
            _supportService = supportService;
            _adminService = adminService;
            _logger = logger;
            _tagService = tagService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var user = await _adminService.GetUserAsync(userId);
            return Ok(new
            {
                StatusCode = 200,
                Data = user
            });
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("user/username/{username}")]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            var user = await _adminService.GetUserAsyncbyUsername(username);
            return Ok(new
            {
                StatusCode = 200,
                Data = user
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _adminService.GetAllUsersAsync();
            return Ok(new
            {
                StatusCode = 200,
                Data = users
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("supports")]
        public async Task<IActionResult> GetAllSupportRequests()
        {
            var supportRequests = await _adminService.GetAllSupportsAsync();
            return Ok(new
            {
                StatusCode = 200,
                Data = supportRequests
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("support/{id}")]
        public async Task<IActionResult> GetSupportRequestById(int id)
        {
            var supportRequest = await _adminService.GetSupportRequestByIdAsync(id);
            return Ok(new
            {
                StatusCode = 200,
                Data = supportRequest
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("support/delete/{supportId}")]
        public async Task<IActionResult> DeleteSupport(int supportId)
        {
            await _adminService.DeleteSupportAsync(supportId);
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Support request deleted successfully."
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("users/search")]
        public async Task<IActionResult> SearchUsers(string searchTerm)
        {
            try
            {
                var users = await _adminService.SearchUsersAsync(searchTerm);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Data = users
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Error = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("tag/add")]
        public async Task<IActionResult> AddTag([FromBody] CreateTagDto createTagDto)
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
                await _tagService.AddTagAsync(createTagDto);
                return StatusCode(StatusCodes.Status201Created, new
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = "Tag created successfully"
                });
            }
            catch (GlobalAppException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the tag");
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

        [Authorize(Roles = "Admin")]
        [HttpDelete("tag/delete/{id}")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            try
            {
                await _tagService.DeleteTagAsync(id);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Tag deleted successfully."
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
                _logger.LogError(ex, "An unexpected error occurred while deleting the tag");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("tag/all")]
        public async Task<IActionResult> GetAllTags()
        {
            try
            {
                var tags = await _tagService.GetAllTagsAsync();
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Data = tags
                });
            }
            catch (GlobalAppException ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving tags");
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
        [AllowAnonymous]
        public async Task<IActionResult> AdminLogin([FromBody] LoginDto loginDto)
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
                var result = await _adminService.AdminLoginAsync(loginDto);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Data = result
                });
            }
            catch (GlobalAppException ex)
            {
                _logger.LogError(ex, "An error occurred while logging in the admin");
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Error = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while logging in the admin");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("user/ban/{userId}")]
        public async Task<IActionResult> BanUser(string userId)
        {
            try
            {
                await _adminService.BanUserAsync(userId);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "User banned successfully."
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
                _logger.LogError(ex, "An unexpected error occurred while banning the user");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("user/unban/{userId}")]
        public async Task<IActionResult> UnbanUser(string userId)
        {
            try
            {
                await _adminService.UnbanUserAsync(userId);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "User unbanned successfully."
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
                _logger.LogError(ex, "An unexpected error occurred while unbanning the user");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("user/verify/{userId}")]
        public async Task<IActionResult> VerifyUser(string userId)
        {
            try
            {
                await _adminService.VerifyUserAsync(userId);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "User verified successfully."
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
                _logger.LogError(ex, "An unexpected error occurred while verifying the user");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("user/unverify/{userId}")]
        public async Task<IActionResult> UnverifyUser(string userId)
        {
            try
            {
                await _adminService.UnverifyUserAsync(userId);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "User unverifed successfully."
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
                _logger.LogError(ex, "An unexpected error occurred while unverifying the user");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "An unexpected error occurred. Please try again later."
                });
            }
        }
    }
}
