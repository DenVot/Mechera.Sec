using System.Text.Json.Serialization;

namespace Mechera.Sec.Authorization.Entities;
public class UpdatePasswordEntity
{
    [JsonConstructor]
    public UpdatePasswordEntity(int id, string password)
    {
        Id = id;
        Password = password;
    }

    public int Id { get; }
    public string Password { get; }
}
