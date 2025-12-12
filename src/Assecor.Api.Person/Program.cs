using System.Text.Json;
using Assecor.Api.Application;
using Assecor.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(static options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        }
    );

builder.Services.AddOpenApi();

builder.Services.AddMediatR(static cfg => cfg.RegisterServicesFromAssembly(Application.Assembly));

builder.Services.AddInfrastructure(builder.Configuration);

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

app.Run();
