using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TaskProject.Models;

public partial class User
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string NationalCode { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }
    public string? Username { get; set; }

    public string? PasswordHash { get; set; }
}
