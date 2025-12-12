using Assecor.Api.Infrastructure.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Assecor.Api.Infrastructure.Sql;

public class DatabaseInitializerHostedService(
    IServiceProvider serviceProvider,
    IOptionsMonitor<SqlOptions> sqlOptions,
    ILogger<DatabaseInitializerHostedService> logger
) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var options = sqlOptions.CurrentValue;

        if (!options.UseSql)
        {
            logger.LogDebug("SQL usage is disabled in configuration. Skipping database initialization.");

            return;
        }

        logger.LogInformation("Database initialization service starting...");

        using var scope = serviceProvider.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();

        await initializer.InitializeAsync();

        logger.LogInformation("Database initialization service completed");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
