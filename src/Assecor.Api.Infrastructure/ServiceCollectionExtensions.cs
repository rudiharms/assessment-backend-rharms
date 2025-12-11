using Assecor.Api.Application.Abstractions;
using Assecor.Api.Infrastructure.Abstractions;
using Assecor.Api.Infrastructure.CSV;
using Assecor.Api.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Assecor.Api.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CsvOptions>(configuration.GetSection(CsvOptions.SectionName));

        services.AddSingleton<ICsvService, CsvService>();
        services.AddScoped<IPersonRepository, CsvPersonRepository>();

        return services;
    }
}
