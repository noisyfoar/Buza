using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Buza.Dtos;
using Buza.Tests.TestInfrastructure;

namespace Buza.Tests;

public class AdminWorkflowTests
{
    private static readonly byte[] OnePixelPng = Convert.FromBase64String(
        "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+/p9sAAAAASUVORK5CYII=");

    [Fact]
    public async Task Admin_routes_require_jwt_and_login_rejects_bad_password()
    {
        using var factory = new BuzaWebApplicationFactory();
        using var client = factory.CreateClient();

        var unauthorized = await client.GetAsync("/api/admin/products");
        Assert.Equal(HttpStatusCode.Unauthorized, unauthorized.StatusCode);

        var badLogin = await client.PostAsJsonAsync("/api/auth/login", new LoginRequestDto
        {
            Login = "admin",
            Password = "wrong"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, badLogin.StatusCode);

        var token = await AuthTestHelper.LoginAsAdminAsync(client);
        client.UseBearerToken(token);

        var authorized = await client.GetAsync("/api/admin/products");
        Assert.Equal(HttpStatusCode.OK, authorized.StatusCode);
    }

    [Fact]
    public async Task Admin_can_create_update_upload_image_hide_and_delete_product()
    {
        using var factory = new BuzaWebApplicationFactory();
        using var client = factory.CreateClient();
        var token = await AuthTestHelper.LoginAsAdminAsync(client);
        client.UseBearerToken(token);

        var createResponse = await client.PostAsJsonAsync("/api/admin/products", new ProductUpsertDto
        {
            Name = "Буза тестовая",
            ShortDescription = "Позиция для автотеста",
            Description = "Полное описание тестового товара",
            Composition = "Зерновая основа, вода, закваска",
            Volume = "0,33 л",
            Price = 199,
            IsActive = true
        });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<ProductDetailsDto>();
        Assert.NotNull(created);

        using var multipart = new MultipartFormDataContent();
        var imageContent = new ByteArrayContent(OnePixelPng);
        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
        multipart.Add(imageContent, "file", "test-product.png");

        var uploadResponse = await client.PostAsync($"/api/admin/products/{created.Id}/images", multipart);
        uploadResponse.EnsureSuccessStatusCode();
        var uploadedImage = await uploadResponse.Content.ReadFromJsonAsync<ProductImageDto>();

        Assert.NotNull(uploadedImage);
        Assert.StartsWith("/uploads/", uploadedImage.FilePath);

        var updateResponse = await client.PutAsJsonAsync($"/api/admin/products/{created.Id}", new ProductUpsertDto
        {
            Name = "Буза тестовая обновленная",
            ShortDescription = "Обновленное краткое описание",
            Description = "Обновленное полное описание",
            Composition = "Овес, вода, закваска",
            Volume = "0,5 л",
            Price = 250,
            IsActive = false
        });

        updateResponse.EnsureSuccessStatusCode();
        var updated = await updateResponse.Content.ReadFromJsonAsync<ProductDetailsDto>();

        Assert.NotNull(updated);
        Assert.False(updated.IsActive);
        Assert.Equal(250, updated.Price);
        Assert.Contains(updated.Images, image => image.FilePath == uploadedImage.FilePath);

        var publicProducts = await client.GetFromJsonAsync<List<ProductSummaryDto>>("/api/products");
        Assert.NotNull(publicProducts);
        Assert.DoesNotContain(publicProducts, product => product.Id == created.Id);

        var deleteResponse = await client.DeleteAsync($"/api/admin/products/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var afterDelete = await client.GetAsync($"/api/admin/products/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, afterDelete.StatusCode);
    }

    [Fact]
    public async Task Admin_can_update_site_settings()
    {
        using var factory = new BuzaWebApplicationFactory();
        using var client = factory.CreateClient();
        var token = await AuthTestHelper.LoginAsAdminAsync(client);
        client.UseBearerToken(token);

        var settings = await client.GetFromJsonAsync<SiteSettingsDto>("/api/settings");
        Assert.NotNull(settings);

        var settingsResponse = await client.PutAsJsonAsync("/api/admin/settings", new UpdateSiteSettingsDto
        {
            ProjectName = "Буза автотест",
            LogoPath = settings.LogoPath,
            Phone = "+7 (347) 000-00-00",
            Email = "autotest@example.com",
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
        });

        settingsResponse.EnsureSuccessStatusCode();
        var publicSettings = await client.GetFromJsonAsync<SiteSettingsDto>("/api/settings");

        Assert.NotNull(publicSettings);
        Assert.Equal("Буза автотест", publicSettings.ProjectName);
        Assert.Equal("autotest@example.com", publicSettings.Email);
    }
}
