namespace WorkplaceTasks.Application.DTOs
{
    public class UpdateProfileDto
    {
        public string Email { get; set; } = string.Empty;
        public string CurrentPassword { get; set; } = string.Empty;
    }
}