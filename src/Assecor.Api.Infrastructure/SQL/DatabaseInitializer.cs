using Assecor.Api.Infrastructure.Sql.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Assecor.Api.Infrastructure.Sql;

public class DatabaseInitializer(PersonDbContext context, ILogger<DatabaseInitializer> logger)
{
    public async Task InitializeAsync()
    {
        try
        {
            await context.Database.EnsureCreatedAsync();

            if (!await context.Persons.AnyAsync())
            {
                await SeedData();
            }
            else
            {
                logger.LogInformation("Database already contains data, skipping seed");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database");

            throw;
        }
    }

    private async Task SeedData()
    {
        if (!await context.Persons.AnyAsync())
        {
            logger.LogInformation("Seeding database with initial data...");

            var personResults = new[]
            {
                PersonEntity.Create("Hans", "Müller", "67742", "Lauterecken", 1),
                PersonEntity.Create("Peter", "Petersen", "18439", "Stralsund", 2),
                PersonEntity.Create("Johnny", "Johnson", "88888", "madeup", 3),
                PersonEntity.Create("Milly", "Millenium", "77777", "madeuptoo", 4),
                PersonEntity.Create("Jonas", "Müller", "32323", "Hansstadt", 5),
                PersonEntity.Create("Tastatur", "Fujitsu", "42342", "Japan", 6),
                PersonEntity.Create("Anders", "Andersson", "32132", "Schweden", 2),
                PersonEntity.Create("Gerda", "Gerber", "76535", "Woanders", 3),
                PersonEntity.Create("Klaus", "Klaussen", "43246", "Hierach", 2)
            };

            var persons = new List<PersonEntity>();

            foreach (var personResult in personResults)
            {
                if (personResult.IsFailure)
                {
                    logger.LogError(
                        "Failed to create person entity for seeding: {ErrorCode} - {ErrorMessage}",
                        personResult.Error.Code,
                        personResult.Error.Message
                    );

                    continue;
                }

                persons.Add(personResult.Value);
            }

            await context.Persons.AddRangeAsync(persons);
            await context.SaveChangesAsync();

            logger.LogInformation("Database seeded successfully");
        }
    }
}
