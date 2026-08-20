using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskProject.Models;

public partial class Task
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }
    public DateOnly? DueDate { get; set; }

    public TasksStatus Status { get; set; } = TasksStatus.Pending;
}
