using Mechera.Sec.Data.Models;
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
