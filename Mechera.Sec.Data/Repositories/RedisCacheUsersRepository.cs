using Mechera.Sec.Data.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

namespace Mechera.Sec.Data.Repositories;

public class RedisCacheUsersRepository : IUsersRepository, IDisposable
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
        await _originalRepository.AddAsync(entity);        
    }

    public void Dispose()
    {
        if (_originalRepository is IDisposable disposableRepo) disposableRepo.Dispose();
        _dbContext.Dispose();        
    }

    public IQueryable<User> GetAll() => _originalRepository.GetAll();   

    public Task<User?> GetAsync(long id) =>
        GetUserByIdentifierAsync(id.ToString(), 
            (identifier) => _originalRepository.GetAsync(long.Parse(identifier)));

    public Task<User?> GetUserByUsernameAsync(string username) => 
        GetUserByIdentifierAsync(username, _originalRepository.GetUserByUsernameAsync);

    private async Task<User?> GetUserByIdentifierAsync(string identifier, Func<string, Task<User?>> getUserFromOriginal)
    {
        var cachedResult = await _cache.GetAsync(identifier);

        User target;

        if (cachedResult == null)
        {
            var resultFromOriginal = await getUserFromOriginal(identifier);

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
        var jsonBytes = await _cache.GetAsync(entity.Id.ToString());

        if (jsonBytes != null)
        {
            var cachedUser = JsonSerializer.Deserialize<User>(jsonBytes);

            if (cachedUser!.Username != null)
            {
                await RemoveEntityFromCacheAsync(cachedUser!);
            }            
        }

        await _originalRepository.RemoveAsync(entity);
    }

    public async Task SaveChangesAsync()
    {       
        if (_dbContext.ChangeTracker.HasChanges())
        {
            foreach(var userEntry in _dbContext.ChangeTracker.Entries<User>())
            {
                if (userEntry.Entity.Username == null) continue;
                await LoadEntityToCacheAsync(userEntry.Entity);
            }
        }

        await _originalRepository.SaveChangesAsync();
    }

    private async Task LoadEntityToCacheAsync(User user)
    {
        var jsonBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(user));

        await _cache.SetAsync(user.Id.ToString(), jsonBytes);
        await _cache.SetAsync(user.Username, jsonBytes);
    }

    private async Task RemoveEntityFromCacheAsync(User user)
    {
        await _cache.RemoveAsync(user.Username);
        await _cache.RemoveAsync(user.Id.ToString());
    }
}
