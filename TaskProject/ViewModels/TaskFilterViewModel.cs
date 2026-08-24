namespace TaskProject.ViewModels
{
    public class TaskFilterViewModel
    {
        public string? Title { get; set; }
        public TasksStatus? Status { get; set; }
        public DateOnly? DueDate { get; set; }
        public int DueDateSearchType { get; set; } = 0;

        public string? SortColumn { get; set; }
        public bool SortDescending { get; set; }

        public int Page { get; set; } = 0;
        public int ItemCount { get; set; } = 2;

        public int TotalItems { get; set; }

        public int TotalPages { get; set; }
    }
}
