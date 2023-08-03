using Mechera.Sec.Data.Models;
using Mechera.Sec.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mechera.Sec.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMecheraSecData(this IServiceCollection serviceCollection,
        string dbConnectionString, 
        string redisConnectionString) =>
#if DEBUG
        serviceCollection
            .AddScoped<IUsersRepository, EfUsersRepository>()
            .Decorate<IUsersRepository, RedisCacheUsersRepository>()
            .AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");                
            })
            .AddDbContext<MecheraDbContext>(dbContextOptions => dbContextOptions
                .UseMySql(configuration.GetConnectionString("MecheraSecDB"), new MySqlServerVersion(new Version(8, 0, 31)))
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors());
#else
        serviceCollection.AddScoped<IUsersRepository, EfUsersRepository>()
            .Decorate<IUsersRepository, RedisCacheUsersRepository>()
            .AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
            })
            .AddDbContext<MecheraDbContext>(dbContextOptions => dbContextOptions
                .UseMySql(dbConnectionString, new MySqlServerVersion(new Version(8, 0, 31))));
#endif
}
