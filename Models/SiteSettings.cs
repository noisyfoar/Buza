using System.ComponentModel.DataAnnotations;

namespace Buza.Models;

public class SiteSettings
{
    public int Id { get; set; }

    [MaxLength(160)]
    public string ProjectName { get; set; } = string.Empty;

    [MaxLength(300)]
    public string LogoPath { get; set; } = string.Empty;

    [MaxLength(40)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(160)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(300)]
    public string Address { get; set; } = string.Empty;

    public string SocialLinks { get; set; } = string.Empty;

    public string FoundationText { get; set; } = string.Empty;

    [MaxLength(300)]
    public string FoundationUrl { get; set; } = string.Empty;

    [MaxLength(300)]
    public string FederalProjectUrl { get; set; } = string.Empty;

    [MaxLength(300)]
    public string MainImagePath { get; set; } = string.Empty;

    [MaxLength(220)]
    public string HeroTitle { get; set; } = string.Empty;

    public string HeroDescription { get; set; } = string.Empty;

    public string ProductHistory { get; set; } = string.Empty;

    public string ProductDescription { get; set; } = string.Empty;

    public string ProductComposition { get; set; } = string.Empty;

    public string ProductBenefits { get; set; } = string.Empty;

    public string TechnologyText { get; set; } = string.Empty;

    public string ProductionSteps { get; set; } = string.Empty;

    public string RawMaterials { get; set; } = string.Empty;

    public string StartupDescription { get; set; } = string.Empty;

    public string ProjectGoals { get; set; } = string.Empty;

    public string DevelopmentStages { get; set; } = string.Empty;

    public string TeamInfo { get; set; } = string.Empty;

    public string DevelopmentPlans { get; set; } = string.Empty;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
