using Buza.Models;

namespace Buza.Repositories;

public interface IAdminUserRepository
{
    Task<AdminUser?> GetByLoginAsync(string login);
}
