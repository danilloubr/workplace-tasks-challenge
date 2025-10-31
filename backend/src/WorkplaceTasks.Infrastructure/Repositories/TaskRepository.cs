using Microsoft.EntityFrameworkCore;
using WorkplaceTasks.Application.Interfaces;
using WorkplaceTasks.Infrastructure.Data;

namespace WorkplaceTasks.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;

        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Domain.Entities.Task?> GetByIdAsync(Guid id)
        {
            return await _context.Tasks.FindAsync(id);
        }

        public async Task<IEnumerable<Domain.Entities.Task>> GetAllAsync()
        {
            return await _context.Tasks.ToListAsync();
        }

        public async Task<Domain.Entities.Task> AddAsync(Domain.Entities.Task task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task UpdateAsync(Domain.Entities.Task task)
        {
            _context.Entry(task).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Domain.Entities.Task task)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }
    }
}