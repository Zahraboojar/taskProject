using System;
using System.Collections.Generic;

namespace TaskProject.Models;

public partial class TaskUser
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int TaskId { get; set; }

    public virtual Task Task { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
