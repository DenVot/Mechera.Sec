﻿using Mechera.Sec.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Mechera.Sec.Data.Repositories;

/// <summary>
/// Реализация <see cref="IUsersRepository"/> с помощью EF Core
/// </summary>
public class EfUsersRepository : IUsersRepository
{
    private readonly DbSet<User> _users;
    private readonly DbContext _dbContext;

    public EfUsersRepository(DbContext dbContext)
    {
        _users = dbContext.Set<User>();
        _dbContext = dbContext;
    }

    /// <inheritdoc/>    
    public Task AddAsync(User entity) => _users.AddAsync(entity).AsTask();

    /// <inheritdoc/>    
    public Task<User?> GetAsync(string username) => _users.FindAsync(username).AsTask();

    /// <inheritdoc/>    
    public Task RemoveAsync(User entity)
    {
        _users.Remove(entity);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>    
    public Task SaveChangesAsync() => _dbContext.SaveChangesAsync();
}
