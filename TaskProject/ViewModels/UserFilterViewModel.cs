namespace TaskProject.ViewModels
{
    public class UserFilterViewModel : BaseFilterViewModel
    {
        public string? FullName { get; set; }
        public string? NationalCode { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }
        public string? Username { get; set; }
    }
}
