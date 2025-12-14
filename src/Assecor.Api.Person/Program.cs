using System.Text.Json;
using Assecor.Api.Application;
using Assecor.Api.Infrastructure;
using Assecor.Api.Person.Filter;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ProblemDetailsLoggingFilter>();

builder.Services.AddControllers(static options =>
        {
            options.Filters.AddService<ProblemDetailsLoggingFilter>();
        }
    )
    .AddJsonOptions(static options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        }
    );

builder.Services.AddOpenApi();

builder.Services.AddMediatR(static cfg => cfg.RegisterServicesFromAssembly(Application.Assembly));

builder.Services.AddInfrastructure(builder.Configuration);

builder.Host.UseSerilog(static (hostContext, services, configuration)
                            => configuration.ReadFrom.Configuration(hostContext.Configuration).ReadFrom.Services(services)
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(static options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "v1");
        }
    );
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("App is starting");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "App start-up failed");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program
{ }
