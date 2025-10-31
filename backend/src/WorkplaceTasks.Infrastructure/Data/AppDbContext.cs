using Microsoft.EntityFrameworkCore;
using WorkplaceTasks.Domain.Entities;
using WorkplaceTasks.Domain.Enums;

namespace WorkplaceTasks.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Domain.Entities.Task> Tasks { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var adminId = Guid.NewGuid();
            var managerId = Guid.NewGuid();
            var memberId = Guid.NewGuid();

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = adminId,
                    Email = "admin@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Role = UserRole.Admin,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = managerId,
                    Email = "manager@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("manager123"),
                    Role = UserRole.Manager,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = memberId,
                    Email = "member@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("member123"),
                    Role = UserRole.Member,
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}