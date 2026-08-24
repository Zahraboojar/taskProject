namespace TaskProject.ViewModels
{
    public class BaseFilterViewModel
    {
        public string? SortColumn { get; set; }
        public bool SortDescending { get; set; }

        public int Page { get; set; } = 0;
        public int ItemCount { get; set; } = 2;

        public int TotalItems { get; set; }

        public int TotalPages { get; set; }
    }
}
