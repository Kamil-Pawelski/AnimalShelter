using AnimalShelter.App.Commands;
using AnimalShelter.Domain;
using AnimalShelter.Domain.UserEntities;
using AnimalShelter.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AnimalShelter.Tests.Controller;

public class AccountHandlerTests
{
    private readonly RegisterCommandHandler _registerCommandHandler;
    private readonly ApplicationDbContext _context;

    public AccountHandlerTests()
    {

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
           .UseInMemoryDatabase(databaseName: "AnimalShelterTest")
           .Options;

        _context = new ApplicationDbContext(options);

        var accountRepository = new AccountRepository(_context);
        var passwordHasher = new PasswordHasher<User>();

        _registerCommandHandler = new RegisterCommandHandler(accountRepository, passwordHasher);

    }

    [Fact]
    public async Task Register_OK()
    {
        var command = new RegisterCommand("TestUser", "Password123!", "testaccount@test.com");

        var result =  await _registerCommandHandler.Handle(command, CancellationToken.None);
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == command.Email);

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(command.Email, user?.Email);
        Assert.Equal("User registered successfully", result.Message);
    }

    [Fact]
    public async Task Register_BadRquest()
    {
        var seedUser = new User
        {
            Username = "TestUser",
            Password = "Password123!",
            Email = "test@test.com"
        };

        _context.Add(seedUser);
        await _context.SaveChangesAsync();

        var command = new RegisterCommand("TestUser", "Password123!", "test@example.com");

        var result = await _registerCommandHandler.Handle(command, CancellationToken.None);

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal("User registered successfully", result.Message);
    }
}