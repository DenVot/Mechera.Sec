﻿namespace Mechera.Sec.Data.Models;

public partial class User
{
    public string Username { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = null!;

    public ulong IsRoot { get; set; }
}