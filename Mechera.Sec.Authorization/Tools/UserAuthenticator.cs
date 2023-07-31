using Mechera.Sec.Data.Models;
using Mechera.Sec.Data.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace Mechera.Sec.Authorization.Tools;

public class UserAuthenticator : IUserAuthenticator, IDisposable
{
    private readonly IUsersRepository _usersRepository;
    private readonly SHA256 _passwordHasher;

    public UserAuthenticator(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
        _passwordHasher = SHA256.Create();
    }

    public async Task<User?> AuthenticateAsync(string username, string password)
    {
        var targetUser = await _usersRepository.GetUserByUsernameAsync(username);
        if (targetUser == null) return null;

        var targetHash = targetUser.PasswordHash;
        var passwordHash = _passwordHasher.ComputeHash(Encoding.UTF8.GetBytes(password));

        for(var i = 0; i < targetHash.Length; i++)
        {
            if (passwordHash[i] != targetHash[i]) return null;
        }

        return targetUser;
    } 

    public void Dispose()
    {
        _passwordHasher.Dispose();
    }
}
