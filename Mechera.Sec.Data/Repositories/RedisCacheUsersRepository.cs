using Mechera.Sec.Data.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

namespace Mechera.Sec.Data.Repositories;

/*
 Структура кэша:
    ID -> JSON пользователя
    Username -> ID пользователя, по которому можно найти его
 */
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

    public async Task<User?> GetAsync(long id)
    {
        var cachedResult = await _cache.GetAsync(id.ToString());

        User target;

        if (cachedResult == null)
        {
            var resultFromOriginal = await _originalRepository.GetAsync(id);

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

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        var cachedIdBytes = await _cache.GetAsync(username); 

        if (cachedIdBytes == null)
        {
            var resultFromOriginal = await _originalRepository.GetUserByUsernameAsync(username);
            if (resultFromOriginal == null) return null;            

            await LoadEntityToCacheAsync(resultFromOriginal);

            return resultFromOriginal;
        }
        else 
        { 
            var cachedId = long.Parse(Encoding.UTF8.GetString(cachedIdBytes));

            return await GetAsync(cachedId);
        }
    }

    public async Task RemoveAsync(User entity)
    {
        var jsonBytes = await _cache.GetAsync(entity.Id.ToString());

        if (jsonBytes != null)
        {
            var cachedUser = JsonSerializer.Deserialize<User>(jsonBytes);

            await RemoveEntityFromCacheAsync(cachedUser!);            
        }

        await _originalRepository.RemoveAsync(entity);
    }

    public Task SaveChangesAsync() => _originalRepository.SaveChangesAsync();

    private async Task LoadEntityToCacheAsync(User user)
    {
        var jsonBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(user));
        var idBytes = Encoding.UTF8.GetBytes(user.Id.ToString());

        await _cache.SetAsync(user.Id.ToString(), jsonBytes);
        await _cache.SetAsync(user.Username, idBytes);
    }

    private async Task RemoveEntityFromCacheAsync(User user)
    {
        await _cache.RemoveAsync(user.Username);
        await _cache.RemoveAsync(user.Id.ToString());
    }
}
