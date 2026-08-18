using Microsoft.AspNetCore.Mvc.Rendering;
using TaskProject.Models;

namespace TaskProject.ViewModels
{
    public class UserViewModel: User
    {
        public List<int> SelectedTaskIds { get; set; } = new();

        public List<SelectListItem> Tasks { get; set; } = new();
    }
}
