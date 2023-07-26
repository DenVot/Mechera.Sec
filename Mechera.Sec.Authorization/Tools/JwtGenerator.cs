using Mechera.Sec.Data.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Mechera.Sec.Authorization.Tools;

public class JwtGenerator : IJwtGenerator
{
    private readonly JwtSecurityTokenHandler _jwtHandler = new();
    private readonly string? _jwtIssuer;
    private readonly string? _jwtAudience;
    private readonly int _jwtLifetime;

    public JwtGenerator(IConfiguration configuration)
    {
        _jwtIssuer = configuration.GetValue<string>("Jwt:Issuer");
        _jwtAudience = configuration.GetValue<string>("Jwt:Audience");
        _jwtLifetime = configuration.GetValue<int>("Jwt:Lifetime");
    }

    public string GenerateToken(User user)
    {
        var usernameClaim = new Claim("username", user.Username);
        var roleClaim = new Claim("role", user.IsRoot ? "Root" : "Basic");
        var token = GenerateDefaultToken(new[] { usernameClaim, roleClaim });
        
        return _jwtHandler.WriteToken(token);
    }

    private JwtSecurityToken GenerateDefaultToken(IEnumerable<Claim> claims) =>
        new(_jwtIssuer, _jwtAudience, claims, expires: DateTime.Now.AddSeconds(_jwtLifetime));        
}
