using Mechera.Sec.Data.Models;

namespace Mechera.Sec.Authorization.Tools;

public interface IUserAuthenticator
{
    Task<User?> AuthenticateAsync(string username, string password);
}
