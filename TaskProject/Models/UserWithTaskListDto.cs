using Microsoft.EntityFrameworkCore;

namespace TaskProject.Models
{
    [Keyless]
    public class UserWithTaskListDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = null!;

        public string NationalCode { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public string? Username { get; set; }

        public string? PasswordHash { get; set; }

        public int? TaskId { get; set; }

        public string? TaskTitle { get; set; }
    }
}
