using WorkplaceTasks.Application.DTOs;
using WorkplaceTasks.Domain.Enums;

namespace WorkplaceTasks.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserDto> GetMyProfileAsync(Guid currentUserId);
        Task UpdateProfileAsync(Guid currentUserId, UpdateProfileDto updateProfileDto);
        Task ChangePasswordAsync(Guid currentUserId, ChangePasswordDto changePasswordDto);
        Task<IEnumerable<UserDto>> GetAllUsersAsync(UserRole currentUserRole);
        Task<UserDto> GetUserByIdAsync(Guid id, UserRole currentUserRole);
        Task UpdateUserAsync(Guid id, AdminUserUpdateDto updateUserDto, UserRole currentUserRole);
        Task DeleteUserAsync(Guid id, Guid currentUserId, UserRole currentUserRole);
    }
}