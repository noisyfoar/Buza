using Buza.Data;
using Buza.Models;
using Microsoft.EntityFrameworkCore;

namespace Buza.Repositories;

public class SiteSettingsRepository(ApplicationDbContext db) : ISiteSettingsRepository
{
    public Task<SiteSettings?> GetAsync() => db.SiteSettings.FirstOrDefaultAsync();

    public Task SaveChangesAsync() => db.SaveChangesAsync();
}
