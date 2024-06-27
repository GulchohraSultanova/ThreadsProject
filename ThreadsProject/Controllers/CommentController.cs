using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThreadsProject.Bussiness.DTOs.CommentDto;
using ThreadsProject.Bussiness.Services.Interfaces;
using System.Security.Claims;
using ThreadsProject.Core.GlobalException;

namespace ThreadsProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly ILogger<CommentsController> _logger;

        public CommentsController(ICommentService commentService, ILogger<CommentsController> logger)
        {
            _commentService = commentService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment(CreateCommentDto createCommentDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                _logger.LogError("Unauthorized access attempt in AddComment.");
                return Unauthorized(new
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Error = "Unauthorized"
                });
            }

            try
            {
                await _commentService.AddCommentAsync(createCommentDto, userId);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Comment added successfully."
                });
            }
            catch (GlobalAppException ex)
            {
                _logger.LogError(ex, "GlobalAppException in AddComment: {Message}", ex.Message);
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Error = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in AddComment.");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpDelete("{commentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                _logger.LogError("Unauthorized access attempt in DeleteComment.");
                return Unauthorized(new
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Error = "Unauthorized"
                });
            }

            try
            {
                await _commentService.DeleteCommentAsync(commentId, userId);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Comment deleted successfully."
                });
            }
            catch (GlobalAppException ex)
            {
                _logger.LogError(ex, "GlobalAppException in DeleteComment: {Message}", ex.Message);
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Error = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in DeleteComment.");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpPost("{commentId}/like")]
        [Authorize]
        public async Task<IActionResult> LikeComment(int commentId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                _logger.LogError("Unauthorized access attempt in LikeComment.");
                return Unauthorized(new
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Error = "Unauthorized"
                });
            }

            try
            {
                await _commentService.LikeCommentAsync(commentId, userId);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Comment liked successfully."
                });
            }
            catch (GlobalAppException ex)
            {
                _logger.LogError(ex, "GlobalAppException in LikeComment: {Message}", ex.Message);
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Error = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in LikeComment.");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpDelete("{commentId}/unlike")]
        [Authorize]
        public async Task<IActionResult> UnlikeComment(int commentId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                _logger.LogError("Unauthorized access attempt in UnlikeComment.");
                return Unauthorized(new
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Error = "Unauthorized"
                });
            }

            try
            {
                await _commentService.UnlikeCommentAsync(commentId, userId);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Comment unliked successfully."
                });
            }
            catch (GlobalAppException ex)
            {
                _logger.LogError(ex, "GlobalAppException in UnlikeComment: {Message}", ex.Message);
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Error = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in UnlikeComment.");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpGet("{postId}")]
        public async Task<IActionResult> GetCommentsByPostId(int postId)
        {
            var comments = await _commentService.GetCommentsByPostIdAsync(postId);
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Data = comments
            });
        }
        [HttpGet("user/{userId}/replies")]
        [Authorize]
        public async Task<IActionResult> GetUserRepliesWithPostsByUserId(string userId)
        {
            try
            {
                var commentsWithPosts = await _commentService.GetUserRepliesWithPostsByUserIdAsync(userId);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Data = commentsWithPosts
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

        [HttpGet("replies")]
        [Authorize]
        public async Task<IActionResult> GetMyRepliesWithPosts()
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
                var commentsWithPosts = await _commentService.GetMyRepliesWithPostsAsync(userId);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Data = commentsWithPosts
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
