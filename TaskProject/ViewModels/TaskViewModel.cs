using Microsoft.AspNetCore.Mvc.Rendering;
using TaskProject.Models;

namespace TaskProject.ViewModels
{
    public class TaskViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public TasksStatus Status { get; set; }

        public List<int> SelectedCategoryIds { get; set; } = new();

        public List<SelectListItem> Categories { get; set; } = new();

        public List<int> SelectedUserIds { get; set; } = new();

        public List<SelectListItem> Users { get; set; } = new();
    }
}
