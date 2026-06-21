using Buza.Data;
using Buza.Models;
using Microsoft.EntityFrameworkCore;

namespace Buza.Repositories;

public class AdminUserRepository(ApplicationDbContext db) : IAdminUserRepository
{
    public Task<AdminUser?> GetByLoginAsync(string login) =>
        db.AdminUsers.FirstOrDefaultAsync(admin => admin.Login == login);
}
