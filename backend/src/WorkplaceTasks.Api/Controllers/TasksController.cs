using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WorkplaceTasks.Application.DTOs;
using WorkplaceTasks.Application.Interfaces.Services;
using WorkplaceTasks.Domain.Enums;

namespace WorkplaceTasks.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
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

        // GET: /api/tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks()
        {
            var tasks = await _taskService.GetAllTasksAsync();
            return Ok(tasks);
        }

        // POST: /api/tasks
        [HttpPost]
        public async Task<ActionResult<TaskDto>> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            var creatorId = GetUserId();
            var createdTaskDto = await _taskService.CreateTaskAsync(createTaskDto, creatorId);
            return CreatedAtAction(nameof(GetTaskById), new { id = createdTaskDto.Id }, createdTaskDto);
        }

        // GET: /api/tasks/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDto>> GetTaskById(Guid id)
        {
            var taskDto = await _taskService.GetTaskByIdAsync(id);
            return Ok(taskDto);
        }

        // PUT: /api/tasks/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            var userId = GetUserId();
            var userRole = GetUserRole();
            await _taskService.UpdateTaskAsync(id, updateTaskDto, userId, userRole);
            return NoContent();
        }

        // DELETE: /api/tasks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var userId = GetUserId();
            var userRole = GetUserRole();
            await _taskService.DeleteTaskAsync(id, userId, userRole);
            return NoContent();
        }
    }
}