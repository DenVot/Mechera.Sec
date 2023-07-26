using Mechera.Sec.Data.Models;

namespace Mechera.Sec.Authorization.Entities;

public class UserInfoEntity
{
    public UserInfoEntity(string username, string role)
    {
        Username = username;
        Role = role;
    }

    public string Username { get; }
    public string Role { get; }

    public static UserInfoEntity Create(User user) => 
        new(user.Username, user.IsRoot ? "Root" : "Basic");
}
