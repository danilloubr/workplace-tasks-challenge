namespace WorkplaceTasks.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<Domain.Entities.User?> GetByIdAsync(Guid id);
        Task<Domain.Entities.User?> GetByEmailAsync(string email);
        Task<IEnumerable<Domain.Entities.User>> GetAllAsync();
        Task<bool> AnyAsync(string email, Guid excludeId);
        Task UpdateAsync(Domain.Entities.User user);
        Task DeleteAsync(Domain.Entities.User user);
    }
}