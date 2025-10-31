using WorkplaceTasks.Domain.Enums;

namespace WorkplaceTasks.Application.DTOs
{
    public class AdminUserUpdateDto
    {
        public string Email { get; set; } = string.Empty;

        // Agora você pode usar o nome curto:
        public UserRole Role { get; set; }

        public string? Password { get; set; }
    }
}