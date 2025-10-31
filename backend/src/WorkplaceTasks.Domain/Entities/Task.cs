namespace WorkplaceTasks.Domain.Entities
{
    public class Task
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Enums.TaskStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid CreatorId { get; set; }

        public Task()
        {
            Id = Guid.NewGuid();
            Status = Enums.TaskStatus.Pending;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}