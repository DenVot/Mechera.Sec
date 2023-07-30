using Mechera.Sec.Data.Models;
using Mechera.Sec.Data.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace Mechera.Sec.Authorization;

/// <summary>
/// Сервис, который гарантирует наличие root пользователя
/// </summary>
public class DataSetupService : IHostedService
{   
    private const string RootUsername = "root";
    private static readonly byte[] RootDefPasswordHash = SHA256.HashData(Encoding.UTF8.GetBytes(RootUsername));
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DataSetupService> _logger;

    public DataSetupService(IServiceProvider serviceProvider, ILogger<DataSetupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var usersRepository = scope.ServiceProvider.GetRequiredService<IUsersRepository>();
        var rootUser = await usersRepository.GetUserByUsernameAsync(RootUsername);

        if (rootUser != null && rootUser.IsRoot) 
        {
            _logger.LogDebug("Root user found");
            return;
        }

        if (rootUser != null && !rootUser.IsRoot)
        {
            rootUser.IsRoot = true;
            await usersRepository.SaveChangesAsync();

            _logger.LogDebug("Root user hasn't the \"Root\" role");

            return;
        }

        rootUser = new User
        {
            Username = RootUsername,
            PasswordHash = RootDefPasswordHash,
            IsRoot = true
        };

        await usersRepository.AddAsync(rootUser);
        await usersRepository.SaveChangesAsync();

        if (usersRepository is IDisposable disposableRepo) disposableRepo.Dispose();
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
