using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.Services.Interfaces;
using ThreadsProject.Core.GlobalException;
using System;

namespace ThreadsProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private readonly ILikeService _likeService;

        public LikeController(ILikeService likeService)
        {
            _likeService = likeService;
        }

        [HttpPost("like/{postId}")]
        [Authorize]
        public async Task<IActionResult> Like(int postId)
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
                await _likeService.LikePostAsync(postId, userId);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Post liked successfully."
                });
            }
            catch (GlobalAppException ex)
            {
                Console.WriteLine($"GlobalAppException: {ex.Message}");
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Error = ex.Message
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "An unexpected error occurred. Please try again later."
                });
            }
        }

        [HttpPost("unlike/{postId}")]
        [Authorize]
        public async Task<IActionResult> Unlike(int postId)
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
                await _likeService.UnlikePostAsync(postId, userId);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Post unliked successfully."
                });
            }
            catch (GlobalAppException ex)
            {
                Console.WriteLine($"GlobalAppException: {ex.Message}");
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Error = ex.Message
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "An unexpected error occurred. Please try again later."
                });
            }
        }
    }
}
