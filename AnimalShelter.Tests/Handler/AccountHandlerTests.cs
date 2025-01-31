using AnimalShelter.App.Commands;
using AnimalShelter.Constants;
using AnimalShelter.Domain;
using AnimalShelter.Domain.UserEntities;
using AnimalShelter.Infrastructure.Repositories;
using AnimalShelter.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace AnimalShelter.Tests.Handler;

public class AccountHandlerTests
{
    private readonly RegisterCommandHandler _registerCommandHandler;
    private readonly LoginCommandHandler _loginCommandHandler;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    // TODO: Move conf to own file
    public AccountHandlerTests()
    {
        _configuration = new ConfigurationBuilder()
                 .AddJsonFile("appsettings.Test.json")
                 .Build();

        AppConfigurationConstants.Initialize(_configuration);

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
           .UseInMemoryDatabase(databaseName: "AnimalShelterTest")
           .Options;

        _context = new ApplicationDbContext(options);

        var accountRepository = new AccountRepository(_context);
        var passwordHasher = new PasswordHasher<User>();
        var jwtService = new JWTService(accountRepository);

        _registerCommandHandler = new RegisterCommandHandler(accountRepository, passwordHasher);
        _loginCommandHandler = new LoginCommandHandler(accountRepository, passwordHasher, jwtService);

        SeedData(passwordHasher);
    }

    private void SeedData(IPasswordHasher<User> passwordHasher)
    {
        var user = new User
        {
            Username = "TestUser",
            Email = "testaccount@test.com",
        };

        user.Password = passwordHasher.HashPassword(user, "Password123!");
        var empRole = new Role
        {
            Name = RolesConstants.Employee
        };

        var usrRole = new Role
        {
            Name = RolesConstants.User
        };

        _context.Users.Add(user);
        _context.Roles.Add(empRole);
        _context.Roles.Add(usrRole);

        var UserRoles = new UserRole
        {
            RoleId = empRole.Id,
            UserId = user.Id
        };

        _context.UserRoles.Add(UserRoles);
        _context.SaveChanges();
    }

    [Fact]
    public async Task Register_OK()
    {
        var command = new RegisterCommand("NewUser", "Password123!", "new@test.com");

        var result = await _registerCommandHandler.Handle(command, CancellationToken.None);
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == command.Email);

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(command.Email, user?.Email);
        Assert.Equal("User registered successfully", result.Message);
    }

    [Fact]
    public async Task Register_EmailAlreadyExist_BadRquest()
    {
        var command = new RegisterCommand("NewUser", "Password123!", "testaccount@test.com");

        var result = await _registerCommandHandler.Handle(command, CancellationToken.None);

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Equal("User with this email already exists.", result.Message);
    }

    [Fact]
    public async Task Register_UsernameAlreadyExist_BadRquest()
    {
        var command = new RegisterCommand("TestUser", "Password123!", "newaccount@test.com");

        var result = await _registerCommandHandler.Handle(command, CancellationToken.None);

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Equal("User with this username already exists.", result.Message);
    }

    [Fact]
    public async Task Login_Ok()
    {
        var command = new LoginCommand("TestUser", "Password123!");

        var result = await _loginCommandHandler.Handle(command, CancellationToken.None);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(result.Result);
    }

    [Fact]
    public async Task Login_WrongUsername_BadRequest()
    {
        var command = new LoginCommand("Wrong", "Password123!");

        var result = await _loginCommandHandler.Handle(command, CancellationToken.None);
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Equal("The user with the given username does not exist", result.Message);
    }

    [Fact]
    public async Task Login_WrongPassword_BadRequest()
    {
        var command = new LoginCommand("TestUser", "Wrong123!");

        var result = await _loginCommandHandler.Handle(command, CancellationToken.None);
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Equal("Incorrect password.", result.Message);
    }
}