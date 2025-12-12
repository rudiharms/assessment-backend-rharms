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

            var persons = new[]
            {
                new PersonEntity
                {
                    FirstName = "Hans",
                    LastName = "Müller",
                    ZipCode = "67742",
                    City = "Lauterecken",
                    ColorId = 1
                },
                new PersonEntity
                {
                    FirstName = "Peter",
                    LastName = "Petersen",
                    ZipCode = "18439",
                    City = "Stralsund",
                    ColorId = 2
                },
                new PersonEntity
                {
                    FirstName = "Johnny",
                    LastName = "Johnson",
                    ZipCode = "88888",
                    City = "madeup",
                    ColorId = 3
                },
                new PersonEntity
                {
                    FirstName = "Milly",
                    LastName = "Millenium",
                    ZipCode = "77777",
                    City = "madeuptoo",
                    ColorId = 4
                },
                new PersonEntity
                {
                    FirstName = "Jonas",
                    LastName = "Müller",
                    ZipCode = "32323",
                    City = "Hansstadt",
                    ColorId = 5
                },
                new PersonEntity
                {
                    FirstName = "Tastatur",
                    LastName = "Fujitsu",
                    ZipCode = "42342",
                    City = "Japan",
                    ColorId = 6
                },
                new PersonEntity
                {
                    FirstName = "Anders",
                    LastName = "Andersson",
                    ZipCode = "32132",
                    City = "Schweden",
                    ColorId = 2
                },
                new PersonEntity
                {
                    FirstName = "Gerda",
                    LastName = "Gerber",
                    ZipCode = "76535",
                    City = "Woanders",
                    ColorId = 3
                },
                new PersonEntity
                {
                    FirstName = "Klaus",
                    LastName = "Klaussen",
                    ZipCode = "43246",
                    City = "Hierach",
                    ColorId = 2
                }
            };

            await context.Persons.AddRangeAsync(persons);
            await context.SaveChangesAsync();

            logger.LogInformation("Database seeded successfully");
        }
    }
}
