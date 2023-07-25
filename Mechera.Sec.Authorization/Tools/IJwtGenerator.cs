using Mechera.Sec.Data.Models;

namespace Mechera.Sec.Authorization.Tools;

public interface IJwtGenerator
{
    string GenerateToken(User user);
}
