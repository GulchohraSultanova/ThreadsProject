using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.RepostDto;
using ThreadsProject.Bussiness.Services.Interfaces;
using ThreadsProject.Core.GlobalException;

namespace ThreadsProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepostController : ControllerBase
    {
        private readonly IRepostService _repostService;

        public RepostController(IRepostService repostService)
        {
            _repostService = repostService;
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> AddRepost(RepostCreateDto repostCreateDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized(new
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Error = "Unauthorized"
                });
            }

            try
            {
                await _repostService.AddRepostAsync(repostCreateDto, userId);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Repost created successfully."
                });
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
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> RemoveRepost(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized(new
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Error = "Unauthorized"
                });
            }

            try
            {
                await _repostService.RemoveRepostAsync(id, userId);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Repost removed successfully."
                });
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
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpGet("repost/{id}")]
        public async Task<IActionResult> GetRepost(int id)
        {
            try
            {
                var repost = await _repostService.GetRepostByIdAsync(id);
                if (repost == null)
                {
                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Error = "Repost not found."
                    });
                }
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Data = repost
                });
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
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpGet("userReposts")]
        public async Task<IActionResult> GetAllReposts()
        {
            try
            {
                var reposts = await _repostService.GetAllRepostsAsync();
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Data = reposts
                });
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
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "An unexpected error occurred. Please try again later."
                });
            }
        }
        [HttpGet("{userId}/reposts")]
        [Authorize]
        public async Task<IActionResult> GetRepostsByUserId(string userId)
        {
            var requesterId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (requesterId == null)
            {
                return Unauthorized(new
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Error = "Unauthorized"
                });
            }

            try
            {
                var reposts = await _repostService.GetRepostsByUserIdAsync(userId, requesterId);
                if (!reposts.Any())
                {
                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Error = "No reposts found for this user."
                    });
                }

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Data = reposts
                });
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
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "An unexpected error occurred. Please try again later."
                });
            }
        }

    }
}
