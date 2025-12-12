using System.IO.Abstractions;
using Assecor.Api.Application.Abstractions;
using Assecor.Api.Infrastructure.Abstractions;
using Assecor.Api.Infrastructure.Csv;
using Assecor.Api.Infrastructure.Options;
using Assecor.Api.Infrastructure.Sql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Assecor.Api.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CsvOptions>(configuration.GetSection(CsvOptions.SectionName));
        services.Configure<SqlOptions>(configuration.GetSection(SqlOptions.SectionName));

        var sqlOptions = configuration.GetRequiredSection(SqlOptions.SectionName).Get<SqlOptions>();

        if (sqlOptions?.UseSql == true)
        {
            services.AddDbContextFactory<PersonDbContext>(static (provider, optionsBuilder) =>
                {
                    var connectionString = provider.GetRequiredService<IConfiguration>().GetConnectionString(ConnectionStrings.PersonDb);

                    optionsBuilder.UseSqlite(connectionString);
                }
            );

            services.AddScoped<DatabaseInitializer>();
            services.AddHostedService<DatabaseInitializerHostedService>();
            services.AddScoped<IPersonRepository, SqlPersonRepository>();
        }
        else
        {
            services.AddSingleton<ICsvService, CsvService>();
            services.AddScoped<IPersonRepository, CsvPersonRepository>();
        }

        services.AddSingleton<IFileSystem, FileSystem>();

        return services;
    }
}
