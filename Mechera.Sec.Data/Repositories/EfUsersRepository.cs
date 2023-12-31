﻿using Mechera.Sec.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Mechera.Sec.Data.Repositories;

/// <summary>
/// Реализация <see cref="IUsersRepository"/> с помощью EF Core
/// </summary>
public class EfUsersRepository : IUsersRepository, IDisposable
{
    private readonly DbSet<User> _users;
    private readonly MecheraDbContext _dbContext;

    public EfUsersRepository(MecheraDbContext dbContext)
    {
        _users = dbContext.Users;
        _dbContext = dbContext;
    }

    /// <inheritdoc/>    
    public Task AddAsync(User entity) => _users.AddAsync(entity).AsTask();

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    public IQueryable<User> GetAll()
    {
        return _dbContext.Users;
    }

    /// <inheritdoc/>    
    public Task<User?> GetAsync(long id) => _users.FindAsync(id).AsTask();

    public Task<User?> GetUserByUsernameAsync(string username) => 
        _dbContext.Users.FromSqlRaw($"SELECT * FROM user WHERE username = '{username}'").FirstOrDefaultAsync();

    /// <inheritdoc/>    
    public Task RemoveAsync(User entity)
    {
        _users.Remove(entity);

        return Task.CompletedTask;
    }   

    /// <inheritdoc/>    
    public Task SaveChangesAsync() => _dbContext.SaveChangesAsync();
}
