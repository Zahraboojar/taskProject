using System;
using System.Collections.Generic;

namespace TaskProject.Models;

public partial class Task
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public TasksStatus Status { get; set; } = TasksStatus.Pending;

    public virtual ICollection<CategoryTask> CategoryTasks { get; set; } = new List<CategoryTask>();
}
