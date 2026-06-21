using Buza.Dtos;

namespace Buza.Services;

public interface ISiteSettingsService
{
    Task<SiteSettingsDto> GetAsync();
    Task<SiteSettingsDto> UpdateAsync(UpdateSiteSettingsDto dto);
}
