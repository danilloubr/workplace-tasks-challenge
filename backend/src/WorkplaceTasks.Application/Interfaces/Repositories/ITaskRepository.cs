namespace WorkplaceTasks.Application.Interfaces.Repositories
{
    public interface ITaskRepository
    {
        Task<Domain.Entities.Task?> GetByIdAsync(Guid id);
        Task<IEnumerable<Domain.Entities.Task>> GetAllAsync();
        Task<Domain.Entities.Task> AddAsync(Domain.Entities.Task task);
        Task UpdateAsync(Domain.Entities.Task task);
        Task DeleteAsync(Domain.Entities.Task task);
    }
}