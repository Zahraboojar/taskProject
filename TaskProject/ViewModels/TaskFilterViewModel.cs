namespace TaskProject.ViewModels
{
    public class TaskFilterViewModel :  BaseFilterViewModel
    {
        public string? Title { get; set; }
        public TasksStatus? Status { get; set; }
        public DateOnly? DueDate { get; set; }
        public int DueDateSearchType { get; set; } = 0;
    }
}
