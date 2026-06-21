using System.Net;
using AngleSharp.Html.Parser;
using Buza.Tests.TestInfrastructure;

namespace Buza.Tests;

public class FrontendContractTests
{
    [Fact]
    public async Task Public_shell_contains_required_navigation_and_admin_entry()
    {
        using var factory = new BuzaWebApplicationFactory();
        using var client = factory.CreateClient();
        var html = await client.GetStringAsync("/");
        var parser = new HtmlParser();
        var document = await parser.ParseDocumentAsync(html);

        var navLinks = document.QuerySelectorAll(".main-nav a")
            .Select(link => link.GetAttribute("href"))
            .ToHashSet();

        Assert.Contains("#/", navLinks);
        Assert.Contains("#/product", navLinks);
        Assert.Contains("#/catalog", navLinks);
        Assert.Contains("#/technology", navLinks);
        Assert.Contains("#/project", navLinks);
        Assert.Contains("#/contacts", navLinks);
        Assert.NotNull(document.QuerySelector("a[href='/admin.html']"));
        Assert.NotNull(document.QuerySelector("script[src='/js/app.js']"));
        Assert.NotNull(document.QuerySelector("link[href='/css/styles.css']"));
    }

    [Fact]
    public async Task Static_files_for_public_site_and_admin_panel_are_served()
    {
        using var factory = new BuzaWebApplicationFactory();
        using var client = factory.CreateClient();

        var requiredPaths = new[]
        {
            "/",
            "/admin.html",
            "/css/styles.css",
            "/js/app.js",
            "/js/admin.js",
            "/assets/buza-hero.png",
            "/assets/buza-product.png",
            "/assets/buza-process.png"
        };

        foreach (var path in requiredPaths)
        {
            var response = await client.GetAsync(path);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    [Fact]
    public void Client_scripts_cover_required_routes_buttons_and_api_calls()
    {
        var appJs = File.ReadAllText(Path.Combine(TestPaths.ProjectRoot, "wwwroot", "js", "app.js"));
        var adminJs = File.ReadAllText(Path.Combine(TestPaths.ProjectRoot, "wwwroot", "js", "admin.js"));
        var html = File.ReadAllText(Path.Combine(TestPaths.ProjectRoot, "wwwroot", "index.html"));
        var adminHtml = File.ReadAllText(Path.Combine(TestPaths.ProjectRoot, "wwwroot", "admin.html"));

        Assert.Contains("route.path === \"product\"", appJs);
        Assert.Contains("route.path === \"catalog\"", appJs);
        Assert.Contains("route.path.startsWith(\"product/\")", appJs);
        Assert.Contains("route.path === \"technology\"", appJs);
        Assert.Contains("route.path === \"project\"", appJs);
        Assert.Contains("route.path === \"contacts\"", appJs);

        Assert.Contains("data-product-request", appJs);
        Assert.Contains("data-feedback-form", appJs);
        Assert.Contains("/api/products", appJs);
        Assert.Contains("/api/settings", appJs);
        Assert.Contains("/api/feedback", appJs);

        Assert.Contains("/api/auth/login", adminJs);
        Assert.Contains("/api/admin/products", adminJs);
        Assert.Contains("/api/admin/feedback", adminJs);
        Assert.Contains("/api/admin/settings", adminJs);
        Assert.Contains("name=\"image\"", adminJs);

        var combined = $"{html}\n{adminHtml}\n{appJs}\n{adminJs}".ToLowerInvariant();
        Assert.DoesNotContain("react", combined);
        Assert.DoesNotContain("vue", combined);
        Assert.DoesNotContain("angular", combined);
        Assert.DoesNotContain("bootstrap", combined);
    }
}
