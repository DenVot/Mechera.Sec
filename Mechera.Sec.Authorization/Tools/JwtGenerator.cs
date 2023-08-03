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
    private readonly int _jwtLifetime;
    private readonly SymmetricSecurityKey _jwtKey;

    public JwtGenerator(IConfiguration configuration)
    {
#if DEBUG
        _jwtIssuer = configuration["Jwt:Issuer"];        
        _jwtLifetime = configuration.GetValue<int>("Jwt:Lifetime");
        _jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
#else
        _jwtIssuer = EnvConfig.JwtIssuer;

        if (EnvConfig.JwtLifetime == null)
        {
            throw new NullReferenceException();
        }

        _jwtLifetime = EnvConfig.JwtLifetime.Value;
        _jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(EnvConfig.JwtKey!));
#endif
    }

    public string GenerateToken(User user)
    {
        var usernameClaim = new Claim(ClaimTypes.Name, user.Id.ToString());
        var roleClaim = new Claim(ClaimTypes.Role, user.IsRoot ? "Root" : "Basic");
        var token = GenerateDefaultToken(new[] { usernameClaim, roleClaim });
        
        return _jwtHandler.WriteToken(token);
    }

    private JwtSecurityToken GenerateDefaultToken(IEnumerable<Claim> claims) =>
        new(_jwtIssuer,
            claims: claims,
            signingCredentials: new SigningCredentials(_jwtKey, SecurityAlgorithms.HmacSha256), 
            expires: DateTime.Now.AddSeconds(_jwtLifetime));        
}
