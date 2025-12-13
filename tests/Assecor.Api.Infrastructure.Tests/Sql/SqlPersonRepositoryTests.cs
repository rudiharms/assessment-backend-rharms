using Assecor.Api.Domain.Enums;
using Assecor.Api.Domain.Models;
using Assecor.Api.Infrastructure.Sql;
using Assecor.Api.Infrastructure.Sql.Entities;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace Assecor.Api.Infrastructure.Tests.Sql;
//Note for Reviewer : Basic smoke tests, not 100% coverage

public class SqlPersonRepositoryTests
{
    private readonly PersonDbContext _dbContext;
    private readonly SqlPersonRepository _sut;

    public SqlPersonRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<PersonDbContext>().UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}").Options;

        _dbContext = new PersonDbContext(options);
        _sut = new SqlPersonRepository(_dbContext, new NullLogger<SqlPersonRepository>());
    }

    [Fact]
    public async Task GetPersonsAsync_Succeeds_When_Persons_Exist()
    {
        var entity1 = PersonEntity.Create("John", "Doe", "12345", "City", 1, 1).Value;
        var entity2 = PersonEntity.Create("Jane", "Smith", "67890", "Town", 2, 2).Value;

        await _dbContext.Persons.AddRangeAsync(entity1, entity2);
        await _dbContext.SaveChangesAsync();

        var result = await _sut.GetPersonsAsync();

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
            result.Value.First().FirstName.Should().Be("John");
            result.Value.Last().FirstName.Should().Be("Jane");
        }
    }

    [Fact]
    public async Task GetPersonsAsync_Returns_Empty_List_When_No_Persons_Exist()
    {
        var result = await _sut.GetPersonsAsync();

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task GetPersonByIdAsync_Succeeds_When_Person_Exists()
    {
        var entity = PersonEntity.Create("John", "Doe", "12345", "City", 1, 1).Value;
        var entity2 = PersonEntity.Create("John1", "Doe1", "123456", "City1", 2, 2).Value;

        await _dbContext.Persons.AddRangeAsync(entity, entity2);
        await _dbContext.SaveChangesAsync();

        var result = await _sut.GetPersonByIdAsync(1);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().Be(1);
            result.Value.FirstName.Should().Be("John");
            result.Value.LastName.Should().Be("Doe");
        }
    }

    [Fact]
    public async Task GetPersonByIdAsync_Returns_Error_When_Person_Not_Found()
    {
        var result = await _sut.GetPersonByIdAsync(999);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task GetPersonsByColorAsync_By_ColorName_Succeeds()
    {
        var entity1 = PersonEntity.Create("John", "Doe", "12345", "City", 1, 1).Value;
        var entity2 = PersonEntity.Create("Jane", "Smith", "67890", "Town", 1, 2).Value;
        var entity3 = PersonEntity.Create("Bob", "Brown", "11111", "Place", 2, 3).Value;

        await _dbContext.Persons.AddRangeAsync(entity1, entity2, entity3);
        await _dbContext.SaveChangesAsync();

        var result = await _sut.GetPersonsByColorAsync(ColorName.Blau);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(2);
            result.Value.Should().AllSatisfy(static p => p.Color.ColorName.Should().Be(ColorName.Blau));
        }
    }

    [Fact]
    public async Task GetPersonsByColorAsync_By_ColorId_Succeeds()
    {
        var entity1 = PersonEntity.Create("John", "Doe", "12345", "City", 1, 1).Value;
        var entity2 = PersonEntity.Create("Jane", "Smith", "67890", "Town", 2, 2).Value;

        await _dbContext.Persons.AddRangeAsync(entity1, entity2);
        await _dbContext.SaveChangesAsync();

        var result = await _sut.GetPersonsByColorAsync(1);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().HaveCount(1);
            result.Value.First().Color.Id.Should().Be(1);
        }
    }

    [Fact]
    public async Task AddPersonAsync_Succeeds_When_Person_Valid()
    {
        var person = Person.Create(0, "John", "Doe", Address.Create("12345", "City").Value, Color.GetById(1).Value).Value;

        var result = await _sut.AddPersonAsync(person);

        using (new AssertionScope())
        {
            result.IsSuccess.Should().BeTrue();
            result.Value.FirstName.Should().Be("John");
            result.Value.LastName.Should().Be("Doe");
            result.Value.Id.Should().BeGreaterThan(0);

            var savedEntity = await _dbContext.Persons.FirstOrDefaultAsync();
            savedEntity.Should().NotBeNull();
            savedEntity.FirstName.Should().Be("John");
        }
    }
}
