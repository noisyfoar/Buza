using Buza.Dtos;
using Buza.Models;
using Buza.Repositories;

namespace Buza.Services;

public class SiteSettingsService(ISiteSettingsRepository settings) : ISiteSettingsService
{
    public async Task<SiteSettingsDto> GetAsync()
    {
        var siteSettings = await settings.GetAsync()
            ?? throw new InvalidOperationException("Настройки сайта не созданы.");

        return ToDto(siteSettings);
    }

    public async Task<SiteSettingsDto> UpdateAsync(UpdateSiteSettingsDto dto)
    {
        var siteSettings = await settings.GetAsync()
            ?? throw new InvalidOperationException("Настройки сайта не созданы.");

        siteSettings.ProjectName = dto.ProjectName.Trim();
        siteSettings.LogoPath = dto.LogoPath.Trim();
        siteSettings.Phone = dto.Phone.Trim();
        siteSettings.Email = dto.Email.Trim();
        siteSettings.Address = dto.Address.Trim();
        siteSettings.SocialLinks = dto.SocialLinks.Trim();
        siteSettings.FoundationText = dto.FoundationText.Trim();
        siteSettings.FoundationUrl = dto.FoundationUrl.Trim();
        siteSettings.FederalProjectUrl = dto.FederalProjectUrl.Trim();
        siteSettings.MainImagePath = dto.MainImagePath.Trim();
        siteSettings.HeroTitle = dto.HeroTitle.Trim();
        siteSettings.HeroDescription = dto.HeroDescription.Trim();
        siteSettings.ProductHistory = dto.ProductHistory.Trim();
        siteSettings.ProductDescription = dto.ProductDescription.Trim();
        siteSettings.ProductComposition = dto.ProductComposition.Trim();
        siteSettings.ProductBenefits = dto.ProductBenefits.Trim();
        siteSettings.TechnologyText = dto.TechnologyText.Trim();
        siteSettings.ProductionSteps = dto.ProductionSteps.Trim();
        siteSettings.RawMaterials = dto.RawMaterials.Trim();
        siteSettings.StartupDescription = dto.StartupDescription.Trim();
        siteSettings.ProjectGoals = dto.ProjectGoals.Trim();
        siteSettings.DevelopmentStages = dto.DevelopmentStages.Trim();
        siteSettings.TeamInfo = dto.TeamInfo.Trim();
        siteSettings.DevelopmentPlans = dto.DevelopmentPlans.Trim();
        siteSettings.UpdatedAt = DateTime.UtcNow;

        await settings.SaveChangesAsync();

        return ToDto(siteSettings);
    }

    private static SiteSettingsDto ToDto(SiteSettings settings) => new()
    {
        Id = settings.Id,
        ProjectName = settings.ProjectName,
        LogoPath = settings.LogoPath,
        Phone = settings.Phone,
        Email = settings.Email,
        Address = settings.Address,
        SocialLinks = settings.SocialLinks,
        FoundationText = settings.FoundationText,
        FoundationUrl = settings.FoundationUrl,
        FederalProjectUrl = settings.FederalProjectUrl,
        MainImagePath = settings.MainImagePath,
        HeroTitle = settings.HeroTitle,
        HeroDescription = settings.HeroDescription,
        ProductHistory = settings.ProductHistory,
        ProductDescription = settings.ProductDescription,
        ProductComposition = settings.ProductComposition,
        ProductBenefits = settings.ProductBenefits,
        TechnologyText = settings.TechnologyText,
        ProductionSteps = settings.ProductionSteps,
        RawMaterials = settings.RawMaterials,
        StartupDescription = settings.StartupDescription,
        ProjectGoals = settings.ProjectGoals,
        DevelopmentStages = settings.DevelopmentStages,
        TeamInfo = settings.TeamInfo,
        DevelopmentPlans = settings.DevelopmentPlans,
        UpdatedAt = settings.UpdatedAt
    };
}
