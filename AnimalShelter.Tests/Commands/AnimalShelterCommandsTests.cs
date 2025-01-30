using AnimalShelter.App.Commands;
using AnimalShelter.Constants;
using AnimalShelter.Domain;
using AnimalShelter.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace AnimalShelter.Tests.Commands;

public class AnimalShelterCommandsTests
{
    private readonly PostAnimalCommandHandler _postAnimalCommandHandler;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AnimalShelterCommandsTests()
    {
        var inMemorySettings = new Dictionary<string, string> {
            {"JWT:SecretKey", "1d0e043507dd1c288d5b696a2ad8b2e8958cf179b4faf12462f4baccc1e3a1b216d8728f97fcae555e749028c0d5bffa0cd7e95ab5b896bbaee1d93d181b207a"},
            {"JWT:Issuer", "TestIssuer"},
            {"JWT:Audience", "TestAudience"},
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        AppConfigurationConstants.Initialize(_configuration);

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
           .UseInMemoryDatabase(databaseName: "AnimalShelterTest")
           .Options;

        _context = new ApplicationDbContext(options);

        var animalshelterRepository = new AnimalShelterRepository(_context);

        _postAnimalCommandHandler = new PostAnimalCommandHandler(animalshelterRepository);

        SeedData();
    }

    private void SeedData()
    {


    }

    [Fact]
    public async Task AddAnimal_Ok()
    {
        var command = new PostAnimalCommand("Buddy", "Dog", "Golden Retriever", 3, 30);

        var result = await _postAnimalCommandHandler.Handle(command, CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }
}

