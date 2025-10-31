// Em: backend/src/WorkplaceTasks.Application/Services/UserService.cs
using WorkplaceTasks.Application.DTOs;
using WorkplaceTasks.Application.Exceptions;
using WorkplaceTasks.Application.Interfaces.Repositories;
using WorkplaceTasks.Application.Interfaces.Services;
using WorkplaceTasks.Domain.Enums;

namespace WorkplaceTasks.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        private UserDto MapUserToDto(Domain.Entities.User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role.ToString(),
                CreatedAt = user.CreatedAt
            };
        }

        // --- Métodos de Self-Service ---

        public async Task<UserDto> GetMyProfileAsync(Guid currentUserId)
        {
            var user = await _userRepository.GetByIdAsync(currentUserId);
            if (user == null)
            {
                throw new NotFoundException("Usuário não encontrado.");
            }
            return MapUserToDto(user);
        }

        public async Task UpdateProfileAsync(Guid currentUserId, UpdateProfileDto updateProfileDto)
        {
            var user = await _userRepository.GetByIdAsync(currentUserId);
            if (user == null)
            {
                throw new NotFoundException("Usuário não encontrado.");
            }

            var isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(updateProfileDto.CurrentPassword, user.PasswordHash);
            if (!isCurrentPasswordValid)
            {
                throw new BadRequestException("A senha atual está incorreta.");
            }

            if (await _userRepository.AnyAsync(updateProfileDto.Email, currentUserId))
            {
                throw new BadRequestException("Este email já está em uso por outra conta.");
            }

            user.Email = updateProfileDto.Email;
            await _userRepository.UpdateAsync(user);
        }

        public async Task ChangePasswordAsync(Guid currentUserId, ChangePasswordDto changePasswordDto)
        {
            var user = await _userRepository.GetByIdAsync(currentUserId);
            if (user == null)
            {
                throw new NotFoundException("Usuário não encontrado.");
            }

            var isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash);
            if (!isCurrentPasswordValid)
            {
                throw new BadRequestException("A senha atual está incorreta.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            await _userRepository.UpdateAsync(user);
        }

        // --- Métodos de Admin ---

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync(UserRole currentUserRole)
        {
            if (currentUserRole != UserRole.Admin)
            {
                throw new ForbiddenException("Acesso restrito a administradores.");
            }
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapUserToDto);
        }

        public async Task<UserDto> GetUserByIdAsync(Guid id, UserRole currentUserRole)
        {
            if (currentUserRole != UserRole.Admin)
            {
                throw new ForbiddenException("Acesso restrito a administradores.");
            }

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new NotFoundException($"Usuário com ID {id} não encontrado.");
            }
            return MapUserToDto(user);
        }

        public async Task UpdateUserAsync(Guid id, AdminUserUpdateDto updateUserDto, UserRole currentUserRole)
        {
            if (currentUserRole != UserRole.Admin)
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
                throw new BadRequestException("Este email já está em uso por outra conta.");
            }

            user.Email = updateUserDto.Email;
            user.Role = updateUserDto.Role;

            if (!string.IsNullOrEmpty(updateUserDto.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateUserDto.Password);
            }

            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(Guid id, Guid currentUserId, UserRole currentUserRole)
        {
            if (currentUserRole != UserRole.Admin)
            {
                throw new ForbiddenException("Acesso restrito a administradores.");
            }

            if (id == currentUserId)
            {
                throw new BadRequestException("O admin não pode se auto-deletar.");
            }

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new NotFoundException($"Usuário com ID {id} não encontrado.");
            }

            await _userRepository.DeleteAsync(user);
        }
    }
}