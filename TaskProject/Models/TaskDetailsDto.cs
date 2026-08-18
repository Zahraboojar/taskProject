using Microsoft.EntityFrameworkCore;

namespace TaskProject.Models
{
    [Keyless]
    public class TaskDetailsDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public TasksStatus Status { get; set; }

        public int? CategoryId { get; set; }

        public string? CategoryTitle { get; set; }

        public int? UserId { get; set; }

        public string? UserTitle { get; set; }
    }
}
