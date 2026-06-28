using System.ComponentModel.DataAnnotations;

public enum TasksStatus
{
    [Display(Name = "در انتظار انجام")]
    Pending = 0,

    [Display(Name = "در حال انجام")]
    InProgress = 1,

    [Display(Name = "انجام شده")]
    Completed = 2
}