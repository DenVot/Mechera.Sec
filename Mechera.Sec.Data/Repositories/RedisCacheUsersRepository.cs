using Mechera.Sec.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

namespace Mechera.Sec.Data.Repositories;

public class RedisCacheUsersRepository : IUsersRepository
{
    private readonly IDistributedCache _cache;
    private readonly IUsersRepository _originalRepository;
    private readonly MecheraDbContext _dbContext;

    public RedisCacheUsersRepository(IUsersRepository originalRepository, 
        IDistributedCache distributedCache,
        MecheraDbContext dbContext)
    {
        _originalRepository = originalRepository;
        _cache = distributedCache;
        _dbContext = dbContext;
    }    

    public async Task AddAsync(User entity)
    {
        await LoadEntityToCacheAsync(entity);
        await _originalRepository.AddAsync(entity);
    }

    public async Task<User?> GetAsync(string username)
    {
        var cachedResult = await _cache.GetAsync(username);

        User target;

        if (cachedResult == null)
        {
            var resultFromOriginal = await _originalRepository.GetAsync(username);

            if (resultFromOriginal == null) return null;

            target = resultFromOriginal;

            await LoadEntityToCacheAsync(target);
        }
        else
        {
            target = JsonSerializer.Deserialize<User>(cachedResult)!;
            _dbContext.Attach(target);
        }

        return target;
    }

    public async Task RemoveAsync(User entity)
    {
        if (await _cache.GetAsync(entity.Username) != null)
        {
            await _cache.RemoveAsync(entity.Username);
        }

        await _originalRepository.RemoveAsync(entity);
    }

    public async Task SaveChangesAsync()
    {
        if(_dbContext.ChangeTracker.HasChanges())
        {
            foreach(var userEntry in _dbContext.ChangeTracker.Entries<User>())
            {                
                await LoadEntityToCacheAsync(userEntry.Entity);
            }
        }

        await _originalRepository.SaveChangesAsync();
    }

    private async Task LoadEntityToCacheAsync(User user)
    {
        var jsonBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(user));

        await _cache.SetAsync(user.Username, jsonBytes);        
    }
}

