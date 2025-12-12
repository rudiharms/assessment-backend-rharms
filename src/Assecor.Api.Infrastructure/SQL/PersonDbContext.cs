using Assecor.Api.Infrastructure.Sql.Entities;
using Microsoft.EntityFrameworkCore;

namespace Assecor.Api.Infrastructure.Sql;

public class PersonDbContext(DbContextOptions<PersonDbContext> options) : DbContext(options)
{
    public DbSet<PersonEntity> Persons { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PersonEntity>(static entity =>
            {
                entity.ToTable("Persons");
                entity.HasKey(static e => e.Id);
                entity.Property(static e => e.Id).ValueGeneratedOnAdd();
                entity.Property(static e => e.FirstName).IsRequired().HasMaxLength(200);
                entity.Property(static e => e.LastName).IsRequired().HasMaxLength(200);
                entity.Property(static e => e.ZipCode).IsRequired().HasMaxLength(20);
                entity.Property(static e => e.City).IsRequired().HasMaxLength(200);
                entity.Property(static e => e.ColorId).IsRequired();
            }
        );
    }
}
