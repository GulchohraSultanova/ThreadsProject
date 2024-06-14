using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using ThreadsProject.Bussiness.DTOs.PostDto;
using ThreadsProject.Bussiness.Services.Interfaces;
using ThreadsProject.Core.GlobalException;

namespace ThreadsProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly ILogger<PostController> _logger;

        public PostController(IPostService postService, ILogger<PostController> logger)
        {
            _postService = postService;
            _logger = logger;
        }
        [HttpGet("AllPosts")]
        public async Task<ActionResult<IEnumerable<PostGetDto>>> GetAllPosts()
        {
            try
            {
                var posts = await _postService.GetAllPostsAsync(null, "Comments", "Likes", "Reposts", "Images");
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Data = posts
                });
            }
            catch (GlobalAppException ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving posts");
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
        [HttpGet("GetPhoto/{id}")]
        public async Task<ActionResult<PostGetDto>> GetPostById(int id)
        {
            try
            {
                var post = await _postService.GetPostAsync(p => p.Id == id, "Comments", "Likes", "Reposts", "Images");
                if (post == null)
                {
                    return NotFound(new
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Error = "Post not found"
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
                _logger.LogError(ex, "An error occurred while retrieving the post");
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
        [HttpPost("Create")]
        public async Task<IActionResult> AddPost([FromBody] CreatePostDto createPostDto)
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
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new
                    {
                        StatusCode = StatusCodes.Status401Unauthorized,
                        Error = "User is not authorized"
                    });
                }

                await _postService.AddPostAsync(createPostDto, userId);
                return StatusCode(StatusCodes.Status201Created, new
                {
                    StatusCode = StatusCodes.Status201Created,
                    Message = "Post created successfully"
                });
            }
            catch (InvalidOperationException ex)
            {
               
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Error = ex.Message
                });
            }
            catch (GlobalAppException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the post");
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


        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _postService.DeletePostAsync(id, userId);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Post deleted successfully"
                });
            }
            catch (GlobalAppException ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the post");
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
