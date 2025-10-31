using WorkplaceTasks.Application.DTOs;
using WorkplaceTasks.Domain.Enums;

namespace WorkplaceTasks.Application.Interfaces.Services
{
    public interface ITaskService
    {
        Task<TaskDto> GetTaskByIdAsync(Guid taskId);
        Task<IEnumerable<TaskDto>> GetAllTasksAsync();
        Task<TaskDto> CreateTaskAsync(CreateTaskDto createTaskDto, Guid creatorId);
        Task UpdateTaskAsync(Guid taskId, UpdateTaskDto updateTaskDto, Guid currentUserId, UserRole currentUserRole);
        Task DeleteTaskAsync(Guid taskId, Guid currentUserId, UserRole currentUserRole);
    }
}