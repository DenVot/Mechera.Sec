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
}
