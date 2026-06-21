using Buza.Models;

namespace Buza.Repositories;

public interface ISiteSettingsRepository
{
    Task<SiteSettings?> GetAsync();
    Task SaveChangesAsync();
}
