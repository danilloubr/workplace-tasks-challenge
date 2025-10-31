// Em: backend/src/WorkplaceTasks.Application/Services/TaskService.cs
using WorkplaceTasks.Application.DTOs;
using WorkplaceTasks.Application.Exceptions;
using WorkplaceTasks.Application.Interfaces.Repositories;
using WorkplaceTasks.Application.Interfaces.Services;
using WorkplaceTasks.Domain.Enums;

namespace WorkplaceTasks.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        private TaskDto MapTaskToDto(Domain.Entities.Task task)
        {
            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status.ToString(),
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                CreatorId = task.CreatorId
            };
        }

        public async Task<TaskDto> GetTaskByIdAsync(Guid taskId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);

            if (task == null)
            {
                throw new NotFoundException($"Tarefa com ID {taskId} não encontrada.");
            }

            return MapTaskToDto(task);
        }

        public async Task<IEnumerable<TaskDto>> GetAllTasksAsync()
        {
            var tasks = await _taskRepository.GetAllAsync();
            return tasks.Select(MapTaskToDto);
        }

        public async Task<TaskDto> CreateTaskAsync(CreateTaskDto createTaskDto, Guid creatorId)
        {
            var task = new Domain.Entities.Task
            {
                Title = createTaskDto.Title,
                Description = createTaskDto.Description,
                CreatorId = creatorId
            };

            var createdTask = await _taskRepository.AddAsync(task);

            return MapTaskToDto(createdTask);
        }

        public async Task UpdateTaskAsync(Guid taskId, UpdateTaskDto updateTaskDto, Guid currentUserId, UserRole currentUserRole)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);

            if (task == null)
            {
                throw new NotFoundException($"Tarefa com ID {taskId} não encontrada.");
            }
            if (currentUserRole == UserRole.Member && task.CreatorId != currentUserId)
            {
                throw new ForbiddenException("Membros só podem atualizar as próprias tarefas.");
            }
            task.Title = updateTaskDto.Title;
            task.Description = updateTaskDto.Description;
            task.Status = updateTaskDto.Status;
            task.UpdatedAt = DateTime.UtcNow;

            await _taskRepository.UpdateAsync(task);
        }

        public async Task DeleteTaskAsync(Guid taskId, Guid currentUserId, UserRole currentUserRole)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);

            if (task == null)
            {
                throw new NotFoundException($"Tarefa com ID {taskId} não encontrada.");
            }

            if (currentUserRole == UserRole.Member || currentUserRole == UserRole.Manager)
            {
                if (task.CreatorId != currentUserId)
                {
                    throw new ForbiddenException("Managers e Membros só podem deletar as próprias tarefas.");
                }
            }
            await _taskRepository.DeleteAsync(task);
        }
    }
}