using System.Net;
using System.Net.Http.Json;
using Buza.Data;
using Buza.Dtos;
using Buza.Tests.TestInfrastructure;
using Microsoft.EntityFrameworkCore;

namespace Buza.Tests;

public class ApiIntegrationTests
{
    [Fact]
    public async Task First_start_applies_migrations_and_seeds_required_data()
    {
        using var factory = new BuzaWebApplicationFactory();
        using var client = factory.CreateClient();

        var settings = await client.GetFromJsonAsync<SiteSettingsDto>("/api/settings");

        Assert.NotNull(settings);
        Assert.Equal("Напиток здоровья и молодости — буза", settings.ProjectName);
        Assert.True(File.Exists(factory.DatabasePath));

        var dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite($"Data Source={factory.DatabasePath}")
            .Options;
        await using var db = new ApplicationDbContext(dbOptions);
        var admin = await db.AdminUsers.SingleAsync(user => user.Login == "admin");

        Assert.NotEqual("admin123", admin.PasswordHash);
        Assert.Contains("PBKDF2", admin.PasswordHash);
        Assert.True(await db.Products.CountAsync() >= 2);
        Assert.True(await db.SiteSettings.AnyAsync());
    }

    [Fact]
    public async Task Public_product_and_settings_controllers_return_seeded_content()
    {
        using var factory = new BuzaWebApplicationFactory();
        using var client = factory.CreateClient();

        var products = await client.GetFromJsonAsync<List<ProductSummaryDto>>("/api/products");
        Assert.NotNull(products);
        Assert.True(products.Count >= 2);
        Assert.All(products, product => Assert.False(string.IsNullOrWhiteSpace(product.Name)));

        var details = await client.GetFromJsonAsync<ProductDetailsDto>($"/api/products/{products[0].Id}");
        Assert.NotNull(details);
        Assert.Equal(products[0].Id, details.Id);
        Assert.NotEmpty(details.Description);
        Assert.NotEmpty(details.Composition);

        var missing = await client.GetAsync("/api/products/999999");
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);

        var settings = await client.GetFromJsonAsync<SiteSettingsDto>("/api/settings");
        Assert.NotNull(settings);
        Assert.NotEmpty(settings.HeroTitle);
        Assert.NotEmpty(settings.FoundationText);
    }

}
