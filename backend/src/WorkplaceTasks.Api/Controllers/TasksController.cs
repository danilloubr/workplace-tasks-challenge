using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WorkplaceTasks.Application.DTOs;
using WorkplaceTasks.Domain.Enums;
using WorkplaceTasks.Infrastructure.Data;
using WorkplaceTasks.Application.Exceptions;

namespace WorkplaceTasks.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        private Guid GetUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
        }

        private UserRole GetUserRole()
        {
            var userRoleString = User.FindFirstValue(ClaimTypes.Role);
            return Enum.TryParse<UserRole>(userRoleString, out var role) ? role : UserRole.Member;
        }

        // GET: /api/tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks()
        {
            var tasks = await _context.Tasks
                .Select(task => new TaskDto
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    Status = task.Status.ToString(),
                    CreatedAt = task.CreatedAt,
                    UpdatedAt = task.UpdatedAt,
                    CreatorId = task.CreatorId
                })
                .ToListAsync();

            return Ok(tasks);
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

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            var taskDto = new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status.ToString(),
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                CreatorId = task.CreatorId
            };

            return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, taskDto);
        }

        // GET: /api/tasks/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDto>> GetTaskById(Guid id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

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

        // PUT: /api/tasks/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            var task = await _context.Tasks.FindAsync(id);

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
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: /api/tasks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                throw new NotFoundException($"Tarefa com ID {id} não encontrada.");
            }

            var userId = GetUserId();
            var userRole = GetUserRole();

            if ((userRole == UserRole.Member || userRole == UserRole.Manager) && task.CreatorId != userId)
            {
                throw new ForbiddenException("Managers e Membros só podem deletar as próprias tarefas.");
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}