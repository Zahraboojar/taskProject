using System;
using System.Collections.Generic;

namespace TaskProject.Models;

public partial class CategoryTask
{
    public int Id { get; set; }

    public int TaskId { get; set; }

    public int CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Task Task { get; set; } = null!;
}
