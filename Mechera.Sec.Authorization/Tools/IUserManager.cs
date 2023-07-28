using Mechera.Sec.Data.Models;

namespace Mechera.Sec.Authorization.Tools;

/// <summary>
/// Предоставляет интерфейс для манипуляции данными пользователей
/// </summary>
public interface IUserManager
{
    /// <summary>
    /// Создает нового пользователя
    /// </summary>
    /// <param name="username">Имя</param>
    /// <param name="password">Пароль</param>    
    public Task<User?> CreateUserAsync(string username, string password);

    /// <summary>
    /// Обновляет пароль пользователя
    /// </summary>
    /// <param name="newPassword">Новый пароль</param>   
    public Task UpdatePasswordAsync(string username, string newPassword);

    /// <summary>
    /// Удаляет пользователя
    /// </summary>
    /// <param name="username">Имя пользователя</param>    
    public Task RemoveUserAsync(string username);
}
