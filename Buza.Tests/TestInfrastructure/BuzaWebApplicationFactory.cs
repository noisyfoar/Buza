using Buza.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Buza.Tests.TestInfrastructure;

public sealed class BuzaWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _tempDirectory = Path.Combine(Path.GetTempPath(), "buza-tests", Guid.NewGuid().ToString("N"));

    public string DatabasePath => Path.Combine(_tempDirectory, "database.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Directory.CreateDirectory(_tempDirectory);

        builder.UseContentRoot(TestPaths.ProjectRoot);
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FileUpload:MaxSizeBytes"] = "5242880",
                ["FileUpload:AllowedExtensions:0"] = ".jpg",
                ["FileUpload:AllowedExtensions:1"] = ".jpeg",
                ["FileUpload:AllowedExtensions:2"] = ".png",
                ["FileUpload:AllowedExtensions:3"] = ".webp",
                ["FileUpload:AllowedExtensions:4"] = ".gif"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite($"Data Source={DatabasePath}"));
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        try
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, recursive: true);
            }
        }
        catch
        {
            // Best-effort cleanup: Windows may briefly keep SQLite handles open after a test finishes.
        }
    }
}
