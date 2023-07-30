using Mechera.Sec.Data.Models;

namespace Mechera.Sec.Authorization.Entities;

public class UserInfoEntity
{
    public UserInfoEntity(long id, string username, string role)
    {
        Id = id;
        Username = username;
        Role = role;
    }

    public long Id { get; }
    public string Username { get; }
    public string Role { get; }

    public static UserInfoEntity Create(User user) => 
        new(user.Id, user.Username, user.IsRoot ? "Root" : "Basic");
}
