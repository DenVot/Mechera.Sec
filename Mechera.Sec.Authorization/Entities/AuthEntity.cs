using System.Text.Json.Serialization;

namespace Mechera.Sec.Authorization.Entities;

public class AuthEntity
{
    [JsonConstructor]
    public AuthEntity(string username, string password)
    {
        Username = username;
        Password = password;
    }

    public string Username { get; }
    public string Password { get; }
}
