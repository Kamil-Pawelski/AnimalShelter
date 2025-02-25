using AnimalShelter.App.Commands;
using AnimalShelter.App.Queries;
using AnimalShelter.Domain.AnimalShelterEntities;
using AnimalShelter.Domain.Constants;
using AnimalShelter.Domain.Repositores;
using AnimalShelter.Infrastructure.Database;
using AnimalShelter.Infrastructure.Repositories;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace AnimalShelter.Tests.Handler;

public class AnimalShelterHandlerTests
{
    private readonly IAnimalShelterRepository _animalShelterRepository;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AnimalShelterHandlerTests()
    {
        _configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.Test.json")
               .Build();

        AppConfigurationConstants.Initialize(_configuration);

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
           .UseInMemoryDatabase(databaseName: "AnimalShelterTest")
           .Options;

        _context = new ApplicationDbContext(options);

        _animalShelterRepository = new AnimalShelterRepository(_context);

        SeedData();
    }

    private void SeedData()
    {
        var buddy = new Animal
        {
            Name = "Buddy",
            Species = "Dog",
            Breed = "Golden Retriever",
            Age = 3,
            Weight = 30,
            AdoptionStatus = AdoptionStatus.Available
        };

        var whiskers = new Animal
        {
            Name = "Whiskers",
            Species = "Cat",
            Breed = "Maine Coon",
            Age = 2,
            Weight = 6,
            AdoptionStatus = AdoptionStatus.Adopted
        };



        _context.Animals.Add(buddy);
        _context.Animals.Add(whiskers);

        var adopted = new AnimalAdopted
        {
            UserId = 1,
            AnimalId = whiskers.Id,
            AdoptionDate = DateTime.UtcNow,
        };
        _context.AnimalAdoptions.Add(adopted);

        _context.SaveChanges();
    }

    [Fact]
    public async Task AddAnimal_Ok()
    {
        var command = new PostAnimalCommand("NewBuddy", "NewDog", "NewGolden Retriever", 3, 30);
        var postAnimalCommandHandler = new PostAnimalCommandHandler(_animalShelterRepository);

        var result = await postAnimalCommandHandler.Handle(command, CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }

    [Fact]
    public async Task GetAnimal_Ok()
    {
        var command = new GetAnimalQuery(1);
        var getAnimalQueryHandler = new GetAnimalQueryHandler(_animalShelterRepository);

        var result = await getAnimalQueryHandler.Handle(command, CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(result.Result);
    }

    [Fact]
    public async Task GetAnimal_NotFound()
    {
        var command = new GetAnimalQuery(100);
        var getAnimalQueryHandler = new GetAnimalQueryHandler(_animalShelterRepository);

        var result = await getAnimalQueryHandler.Handle(command, CancellationToken.None);

        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        Assert.Null(result.Result);
    }

    [Fact]
    public async Task GetAnimals_RoleUser_Ok()
    {
        var command = new GetAnimalsQuery(RolesConstants.User);
        var getAnimalsQueryHandler = new GetAnimalsQueryHandler(_animalShelterRepository);

        var result = await getAnimalsQueryHandler.Handle(command, CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.DoesNotContain(result.Result!, animal => animal.AdoptionStatus == AdoptionStatus.Adopted);
    }

    [Fact]
    public async Task GetAnimals_RoleEmployee_Ok()
    {
        var command = new GetAnimalsQuery(RolesConstants.Employee);
        var getAnimalsQueryHandler = new GetAnimalsQueryHandler(_animalShelterRepository);

        var result = await getAnimalsQueryHandler.Handle(command, CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(result.Result);
    }

    [Fact]
    public async Task GetAnimals_NoRole_Unauthorized()
    {
        var command = new GetAnimalsQuery("Wrong role");
        var getAnimalsQueryHandler = new GetAnimalsQueryHandler(_animalShelterRepository);

        var result = await getAnimalsQueryHandler.Handle(command, CancellationToken.None);

        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        Assert.Null(result.Result);
    }

    [Fact]
    public async Task DeleteAnimal_Ok()
    {
        var animal = new Animal
        {
            Name = "Animal to delete",
            Breed = "Test",
            Species = "Test",
            Age = 10,
            Weight = 40,
            AdoptionStatus = AdoptionStatus.Available
        };

        await _animalShelterRepository.AddAnimal(animal);
        var id = animal.Id;
        var command = new DeleteAnimalCommand(id);

        var deleteAnimalCommandHandler = new DeleteAnimalCommandHandler(_animalShelterRepository);

        var result = await deleteAnimalCommandHandler.Handle(command, CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal($"Animal with ID {id} has been removed from the shelter.", result.Message);
    }

    [Fact]
    public async Task DeleteAnimal_NotFound()
    {
        var command = new DeleteAnimalCommand(100);

        var deleteAnimalCommandHandler = new DeleteAnimalCommandHandler(_animalShelterRepository);

        var result = await deleteAnimalCommandHandler.Handle(command, CancellationToken.None);

        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        Assert.Equal("The animal with the given ID does not exist.", result.Message);
    }

    [Fact]
    public async Task PostAdopt_Ok()
    {
        var command = new PostAdoptAnimalCommand(1, 1);

        var postAdoptAnimalCommandHandler = new PostAdoptAnimalCommandHandler(_animalShelterRepository);

        var result = await postAdoptAnimalCommandHandler.Handle(command, CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal("Animal Buddy has been adopted by user 1.", result.Message);
    }

    [Fact]
    public async Task PostAdopt_Conflict()
    {
        var command = new PostAdoptAnimalCommand(1, 2);

        var postAdoptAnimalCommandHandler = new PostAdoptAnimalCommandHandler(_animalShelterRepository);

        var result = await postAdoptAnimalCommandHandler.Handle(command, CancellationToken.None);

        Assert.Equal(HttpStatusCode.Conflict, result.StatusCode);
        Assert.Equal("The animal is already adopted.", result.Message);
    }

    [Fact]
    public async Task PostAdopt_NotFound()
    {
        var command = new PostAdoptAnimalCommand(1, 2000);

        var postAdoptAnimalCommandHandler = new PostAdoptAnimalCommandHandler(_animalShelterRepository);

        var result = await postAdoptAnimalCommandHandler.Handle(command, CancellationToken.None);

        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        Assert.Equal("The animal with the given ID does not exist.", result.Message);
    }
    [Fact]
    public async Task GetAdoptedAnimal_Ok()
    {
        var command = new GetAdoptedAnimalsQuery();

        var getAdoptedAnimalsQueryHandler = new GetAdoptedAnimalsQueryHandler(_animalShelterRepository);

        var result = await getAdoptedAnimalsQueryHandler.Handle(command, CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(result.Result);
    }
}