
namespace WorkplaceTasks.Application.DTOs
{
    public class UpdateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Domain.Enums.TaskStatus Status { get; set; } // O usuário pode mudar o status
    }
}