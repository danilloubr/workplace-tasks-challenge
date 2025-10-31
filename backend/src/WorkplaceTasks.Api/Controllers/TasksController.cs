using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WorkplaceTasks.Application.DTOs;
using WorkplaceTasks.Application.Exceptions;
using WorkplaceTasks.Application.Interfaces;
using WorkplaceTasks.Domain.Entities;
using WorkplaceTasks.Domain.Enums;

namespace WorkplaceTasks.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;

        public TasksController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
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

        // GET: /api/tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks()
        {
            var tasks = await _taskRepository.GetAllAsync();

            var taskDtos = tasks.Select(MapTaskToDto);
            return Ok(taskDtos);
        }

        // POST: /api/tasks
        [HttpPost]
        public async Task<ActionResult<TaskDto>> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            var task = new Domain.Entities.Task
            {
                Title = createTaskDto.Title,
                Description = createTaskDto.Description,
                CreatorId = GetUserId()
            };

            var createdTask = await _taskRepository.AddAsync(task);

            var taskDto = MapTaskToDto(createdTask);
            return CreatedAtAction(nameof(GetTaskById), new { id = taskDto.Id }, taskDto);
        }

        // GET: /api/tasks/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDto>> GetTaskById(Guid id)
        {
            var task = await _taskRepository.GetByIdAsync(id);

            if (task == null)
            {
                throw new NotFoundException($"Tarefa com ID {id} não encontrada.");
            }

            return Ok(MapTaskToDto(task));
        }

        // PUT: /api/tasks/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            var task = await _taskRepository.GetByIdAsync(id);

            if (task == null)
            {
                throw new NotFoundException($"Tarefa com ID {id} não encontrada.");
            }

            var userId = GetUserId();
            var userRole = GetUserRole();

            if (userRole == UserRole.Member && task.CreatorId != userId)
            {
                throw new ForbiddenException("Membros só podem atualizar as próprias tarefas.");
            }

            task.Title = updateTaskDto.Title;
            task.Description = updateTaskDto.Description;
            task.Status = updateTaskDto.Status;
            task.UpdatedAt = DateTime.UtcNow;

            await _taskRepository.UpdateAsync(task);
            return NoContent();
        }

        // DELETE: /api/tasks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var task = await _taskRepository.GetByIdAsync(id);

            if (task == null)
            {
                throw new NotFoundException($"Tarefa com ID {id} não encontrada.");
            }

            var userId = GetUserId();
            var userRole = GetUserRole();

            if (userRole == UserRole.Member || userRole == UserRole.Manager)
            {
                if (task.CreatorId != userId)
                {
                    throw new ForbiddenException("Managers e Membros só podem deletar as próprias tarefas.");
                }
            }

            await _taskRepository.DeleteAsync(task);
            return NoContent();
        }
    }
}