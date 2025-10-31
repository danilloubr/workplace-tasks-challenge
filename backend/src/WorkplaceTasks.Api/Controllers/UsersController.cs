using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WorkplaceTasks.Application.DTOs;
using WorkplaceTasks.Application.Exceptions;
using WorkplaceTasks.Domain.Entities;
using WorkplaceTasks.Domain.Enums;
using WorkplaceTasks.Application.Interfaces;
namespace WorkplaceTasks.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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

        private UserDto MapUserToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role.ToString(),
                CreatedAt = user.CreatedAt
            };
        }

        // GET: /api/users/me
        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> GetMyProfile()
        {
            var userId = GetUserId();
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new NotFoundException("Usuário não encontrado.");
            }

            return Ok(MapUserToDto(user));
        }

        // PUT: /api/users/me
        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto updateProfileDto)
        {
            var userId = GetUserId();
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new NotFoundException("Usuário não encontrado.");
            }

            var isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(updateProfileDto.CurrentPassword, user.PasswordHash);
            if (!isCurrentPasswordValid)
            {
                return BadRequest("A senha atual está incorreta.");
            }

            if (await _userRepository.AnyAsync(updateProfileDto.Email, userId))
            {
                return BadRequest("Este email já está em uso por outra conta.");
            }

            user.Email = updateProfileDto.Email;
            await _userRepository.UpdateAsync(user);

            return NoContent();
        }

        // PUT: /api/users/change-password
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var userId = GetUserId();
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new NotFoundException("Usuário não encontrado.");
            }

            var isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash);
            if (!isCurrentPasswordValid)
            {
                return BadRequest("A senha atual está incorreta.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            await _userRepository.UpdateAsync(user);

            return NoContent();
        }

        // GET: /api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            if (GetUserRole() != UserRole.Admin)
            {
                throw new ForbiddenException("Acesso restrito a administradores.");
            }

            var users = await _userRepository.GetAllAsync();

            return Ok(users.Select(MapUserToDto));
        }

        // GET: /api/users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(Guid id)
        {
            if (GetUserRole() != UserRole.Admin)
            {
                throw new ForbiddenException("Acesso restrito a administradores.");
            }

            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                throw new NotFoundException($"Usuário com ID {id} não encontrado.");
            }

            return Ok(MapUserToDto(user));
        }

        // PUT: /api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] AdminUserUpdateDto updateUserDto)
        {
            if (GetUserRole() != UserRole.Admin)
            {
                throw new ForbiddenException("Acesso restrito a administradores.");
            }

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new NotFoundException($"Usuário com ID {id} não encontrado.");
            }

            if (await _userRepository.AnyAsync(updateUserDto.Email, id))
            {
                return BadRequest("Este email já está em uso por outra conta.");
            }

            user.Email = updateUserDto.Email;
            user.Role = updateUserDto.Role;

            if (!string.IsNullOrEmpty(updateUserDto.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateUserDto.Password);
            }

            await _userRepository.UpdateAsync(user);

            return NoContent();
        }


        // DELETE: /api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            if (GetUserRole() != UserRole.Admin)
            {
                throw new ForbiddenException("Acesso restrito a administradores.");
            }

            if (id == GetUserId())
            {
                return BadRequest("O admin não pode se auto-deletar.");
            }

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new NotFoundException($"Usuário com ID {id} não encontrado.");
            }

            await _userRepository.DeleteAsync(user);

            return NoContent();
        }
    }
}