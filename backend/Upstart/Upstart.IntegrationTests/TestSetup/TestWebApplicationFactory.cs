using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Upstart.Persistence.Data;

namespace Upstart.IntegrationTests.TestSetup;

public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    private readonly string _databaseName = $"InMemoryDbForTesting_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the app's DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<UpstartDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add a database context using the same in-memory database name for consistency
            services.AddDbContext<UpstartDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });
        });

        builder.UseEnvironment("Testing");
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Warning);
        });
    }

    public UpstartDbContext GetDbContext()
    {
        var scope = Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<UpstartDbContext>();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            try
            {
                // Clean up the in-memory database if services are still available
                if (Services != null)
                {
                    using var scope = Services.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<UpstartDbContext>();
                    context.Database.EnsureDeleted();
                }
            }
            catch (ObjectDisposedException)
            {
                // Services already disposed, that's fine
            }
        }
        base.Dispose(disposing);
    }
}