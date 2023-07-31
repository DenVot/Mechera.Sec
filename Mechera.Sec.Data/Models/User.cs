using System;
using System.Collections.Generic;

namespace Mechera.Sec.Data.Models;

public partial class User
{
    public string Username { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = null!;

    public bool IsRoot { get; set; }

    public long Id { get; set; }
}
