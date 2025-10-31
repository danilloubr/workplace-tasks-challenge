using Microsoft.EntityFrameworkCore;
using WorkplaceTasks.Application.Interfaces;
using WorkplaceTasks.Infrastructure.Data;

namespace WorkplaceTasks.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Domain.Entities.User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<Domain.Entities.User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<Domain.Entities.User>> GetAllAsync() { return await _context.Users.ToListAsync(); }

        public async Task<bool> AnyAsync(string email, Guid excludeId)
        {
            return await _context.Users.AnyAsync(u => u.Email == email && u.Id != excludeId);
        }

        public async Task UpdateAsync(Domain.Entities.User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Domain.Entities.User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}