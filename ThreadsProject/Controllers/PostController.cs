using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.PostDto;
using ThreadsProject.Bussiness.Exceptions;
using ThreadsProject.Bussiness.Services.Interfaces;
using ThreadsProject.Core.GlobalException;

namespace ThreadsProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly ILogger<PostController> _logger;

        public PostController(IPostService postService, ILogger<PostController> logger)
        {
            _postService = postService;
            _logger = logger;
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
        [Authorize]
        public async Task<IActionResult> GetExplorePosts(int countPerTag = 5)
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
                var posts = await _postService.GetExplorePostsAsync(userId, countPerTag);
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
            catch (PrivateProfileException ex)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Error = ex.Message
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

        [HttpGet("random-public-posts")]
        public async Task<IActionResult> GetRandomPublicPosts([FromQuery] int count = 5)
        {
            var seenPostIds = HttpContext.Session.Get<HashSet<int>>("SeenPostIds") ?? new HashSet<int>();

            try
            {
                var posts = await _postService.GetRandomPublicPostsAsync(count, seenPostIds);

                // Güncellenen seenPostIds setini sessiona kaydedin
                HttpContext.Session.Set("SeenPostIds", seenPostIds);

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Data = posts
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
                _logger.LogError(ex, "An unexpected error occurred.");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "An unexpected error occurred. Please try again later."
                });
            }
        }



        [HttpGet("{userId}/post/{postId}")]
        [Authorize]
        public async Task<IActionResult> GetPostByUserIdAndPostId(string userId, int postId)
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
                var post = await _postService.GetPostByUserIdAndPostIdAsync(userId, postId);
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
        [HttpGet("liked")]
        [Authorize]
        public async Task<IActionResult> GetLikedPostsByUser()
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
                var posts = await _postService.GetLikedPostsByUserAsync(userId);
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
