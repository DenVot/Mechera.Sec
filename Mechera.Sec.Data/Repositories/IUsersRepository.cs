using Mechera.Sec.Data.Models;

namespace Mechera.Sec.Data.Repositories;

/// <summary>
/// Репозиторий, который предоставляет интерфейс для манипуляций с данными пользователей
/// </summary>
public interface IUsersRepository
{
    /// <summary>
    /// Добавляет пользователя
    /// </summary>
    /// <param name="entity">Сущность</param>    
    Task AddAsync(User entity);

    /// <summary>
    /// Получает пользователя по имени
    /// </summary>
    /// <param name="username">Имя</param>
    /// <returns>Null, если пользователь не найден, иначе значение</returns>
    Task<User?> GetAsync(string username);

    /// <summary>
    /// Удаляет пользователя
    /// </summary>
    /// <param name="entity">Сущность</param>  
    Task RemoveAsync(User entity);

    /// <summary>
    /// Сохраняет изменения
    /// </summary>
    /// <returns></returns>
    Task SaveChangesAsync();
}

