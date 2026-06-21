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
        Assert.Contains("Спасибо", settings.FormSuccessMessage);
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

    [Fact]
    public async Task Feedback_controller_validates_input_and_persists_requests()
    {
        using var factory = new BuzaWebApplicationFactory();
        using var client = factory.CreateClient();
        var products = await client.GetFromJsonAsync<List<ProductSummaryDto>>("/api/products");
        Assert.NotNull(products);

        var invalidResponse = await client.PostAsJsonAsync("/api/feedback", new FeedbackCreateDto
        {
            Name = "Нет согласия",
            Phone = "+7 000 000-00-00",
            Email = "bad@example.com",
            ProductId = products[0].Id,
            Message = "Проверка валидации",
            PersonalDataAccepted = false
        });

        Assert.Equal(HttpStatusCode.BadRequest, invalidResponse.StatusCode);

        var createResponse = await client.PostAsJsonAsync("/api/feedback", new FeedbackCreateDto
        {
            Name = "Покупатель",
            Phone = "+7 999 111-22-33",
            Email = "buyer@example.com",
            ProductId = products[0].Id,
            Message = "Хочу заказать бузу",
            PersonalDataAccepted = true
        });

        createResponse.EnsureSuccessStatusCode();
        var body = await createResponse.Content.ReadFromJsonAsync<FeedbackCreateResponse>();

        Assert.NotNull(body);
        Assert.Contains("Спасибо", body.Message);
        Assert.Equal("NEW", body.Request.Status);

        var token = await AuthTestHelper.LoginAsAdminAsync(client);
        client.UseBearerToken(token);
        var adminFeedback = await client.GetFromJsonAsync<List<FeedbackDto>>("/api/admin/feedback");

        Assert.NotNull(adminFeedback);
        Assert.Contains(adminFeedback, request => request.Email == "buyer@example.com" && request.Status == "NEW");
    }

    private sealed record FeedbackCreateResponse(string Message, FeedbackDto Request);
}
