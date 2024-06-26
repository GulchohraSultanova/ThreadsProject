using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.PostDto;
using ThreadsProject.Bussiness.Services.Interfaces;
using ThreadsProject.Core.GlobalException;

namespace ThreadsProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> AddPost(CreatePostDto createPostDto)
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
                await _postService.AddPostAsync(createPostDto, userId);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Post created successfully."
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
        public async Task<IActionResult> DeletePost(int id)
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
                await _postService.DeletePostAsync(id, userId);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Post deleted successfully."
                });
            }
            catch (GlobalAppException ex)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Error = ex.Message,
                    InnerException = ex.InnerException?.Message,
                    StackTrace = ex.StackTrace
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "An unexpected error occurred. Please try again later.",
                    InnerException = ex.InnerException?.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }


        [HttpGet("explore")]
        public async Task<IActionResult> GetExplorePosts()
        {
            var posts = await _postService.GetExplorePostsAsync();
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Data = posts
            });
        }

        [HttpGet("home")]
        [Authorize]
        public async Task<IActionResult> GetHomePosts()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var posts = await _postService.GetHomePostsAsync(userId);
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Data = posts
            });
        }

        [HttpGet("userposts")]
        [Authorize]
        public async Task<IActionResult> GetUserPosts()
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
                var posts = await _postService.GetUserPostsAsync(userId);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Data = posts
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

        [HttpGet("userpost/{id}")]
        public async Task<IActionResult> GetPost(int id)
        {
            try
            {
                var post = await _postService.GetPostAsync(p => p.Id == id);
                if (post == null)
                {
                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Error = "Post not found."
                    });
                }
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Data = post
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

        [HttpGet("user/{userId}/posts")]
        [Authorize]
        public async Task<IActionResult> GetPostsByUserId(string userId)
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
                var posts = await _postService.GetPostsByUserIdAsync(userId, requesterId);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Data = posts
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
