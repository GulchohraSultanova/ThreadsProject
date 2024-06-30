using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

using ThreadsProject.Bussiness.Services.Interfaces;

namespace ThreadsProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserActionController : ControllerBase
    {
        private readonly IUserActionService _userActionService;

        public UserActionController(IUserActionService userActionService)
        {
            _userActionService = userActionService;
        }

        [HttpGet("user-actions")]
        [Authorize]
        public async Task<IActionResult> GetUserActions()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var actions = await _userActionService.GetUserActionsAsync(userId);
            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Data = actions
            });
        }
    }
}
