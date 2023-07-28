using Mechera.Sec.Data.Models;
using Mechera.Sec.Data.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace Mechera.Sec.Authorization.Tools;

public class UserManager : IUserManager
{
    private readonly IUsersRepository _usersRepository;

    public UserManager(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task<User?> CreateUserAsync(string username, string password)
    {
        if (await _usersRepository.GetAsync(username) != null)
        {
            return null;
        }

        var user = new User
        {
            Username = username,
            PasswordHash = SHA256.HashData(Encoding.UTF8.GetBytes(password)),
            IsRoot = false
        };

        await _usersRepository.AddAsync(user);
        await _usersRepository.SaveChangesAsync();

        return user;
    }

    public IEnumerable<User> GetUsers() => _usersRepository.GetAll();

    public async Task RemoveUserAsync(string username)
    {       
        await _usersRepository.RemoveAsync(new User { Username = username });
        await _usersRepository.SaveChangesAsync();
    }

    public async Task UpdatePasswordAsync(string username, string newPassword)
    { 
        var targetUser = await _usersRepository.GetAsync(username);

        ArgumentNullException.ThrowIfNull(targetUser);

        targetUser.PasswordHash = SHA256.HashData(Encoding.UTF8.GetBytes(newPassword));
        await _usersRepository.SaveChangesAsync();
    }
}
