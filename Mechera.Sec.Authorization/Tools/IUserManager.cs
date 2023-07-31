using Mechera.Sec.Data.Models;

namespace Mechera.Sec.Authorization.Tools;

/// <summary>
/// Предоставляет интерфейс для манипуляции данными пользователей
/// </summary>
public interface IUserManager
{
    /// <summary>
    /// Получает всех пользователей
    /// </summary>    
    public IEnumerable<User> GetUsers();

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
    public Task UpdatePasswordAsync(long id, string newPassword);

    /// <summary>
    /// Удаляет пользователя
    /// </summary>
    /// <param name="id">ID пользователя</param>    
    public Task RemoveUserAsync(long id);
}
