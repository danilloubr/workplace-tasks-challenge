using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WorkplaceTasks.Application.DTOs;
using WorkplaceTasks.Application.Interfaces.Services;
using WorkplaceTasks.Domain.Enums;

namespace WorkplaceTasks.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        private Guid GetUserId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
        private UserRole GetUserRole()
        {
            var userRoleString = User.FindFirstValue(ClaimTypes.Role);
            return Enum.TryParse<UserRole>(userRoleString, out var role) ? role : UserRole.Member;
        }

        // GET: /api/users/me
        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> GetMyProfile()
        {
            var userDto = await _userService.GetMyProfileAsync(GetUserId());
            return Ok(userDto);
        }

        // PUT: /api/users/me
        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto updateProfileDto)
        {
            await _userService.UpdateProfileAsync(GetUserId(), updateProfileDto);
            return NoContent();
        }

        // PUT: /api/users/change-password
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            await _userService.ChangePasswordAsync(GetUserId(), changePasswordDto);
            return NoContent();
        }

        // GET: /api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync(GetUserRole());
            return Ok(users);
        }

        // GET: /api/users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id, GetUserRole());
            return Ok(user);
        }

        // PUT: /api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] AdminUserUpdateDto updateUserDto)
        {
            await _userService.UpdateUserAsync(id, updateUserDto, GetUserRole());
            return NoContent();
        }

        // DELETE: /api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await _userService.DeleteUserAsync(id, GetUserId(), GetUserRole());
            return NoContent();
        }
    }
}