using Mechera.Sec.Data.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Mechera.Sec.Authorization.Tools;

public class JwtGenerator : IJwtGenerator
{
    private readonly JwtSecurityTokenHandler _jwtHandler = new();
    private readonly string? _jwtIssuer;
    private readonly string? _jwtAudience;
    private readonly int _jwtLifetime;
    private readonly SymmetricSecurityKey _jwtKey;

    public JwtGenerator(IConfiguration configuration)
    {
        _jwtIssuer = configuration["Jwt:Issuer"];
        _jwtAudience = configuration["Jwt:Audience"];
        _jwtLifetime = configuration.GetValue<int>("Jwt:Lifetime");
        _jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
    }

    public string GenerateToken(User user)
    {
        var usernameClaim = new Claim(ClaimTypes.Name, user.Username);
        var roleClaim = new Claim(ClaimTypes.Role, user.IsRoot ? "Root" : "Basic");
        var token = GenerateDefaultToken(new[] { usernameClaim, roleClaim });
        
        return _jwtHandler.WriteToken(token);
    }

    private JwtSecurityToken GenerateDefaultToken(IEnumerable<Claim> claims) =>
        new(_jwtIssuer, _jwtAudience, claims,
            signingCredentials: new SigningCredentials(_jwtKey, SecurityAlgorithms.HmacSha256), 
            expires: DateTime.Now.AddSeconds(_jwtLifetime));        
}
