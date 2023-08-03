#if RELEASE

namespace Mechera.Sec.Authorization;

internal static class EnvConfig
{
    public static string? JwtKey => Environment.GetEnvironmentVariable("JWT_KEY");
    public static string? JwtIssuer => Environment.GetEnvironmentVariable("JWT_ISSUER");
    public static string? DbConnectionString => Environment.GetEnvironmentVariable("DB_CONN_STRING");
    public static string? RedisConnectionString => Environment.GetEnvironmentVariable("REDIS_CONN_STRING");

    public static int? JwtLifetime
    {
        get 
        {
            var lifetime = Environment.GetEnvironmentVariable("JWT_LIFETIME");

            if (lifetime == null) return null;

            return int.Parse(lifetime);
        }
    }
}
#endif