using AnimalShelter.App.Commands;
using AnimalShelter.App.Routes;
using AnimalShelter.Constants;
using AnimalShelter.Domain.UserEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Net;
using AnimalShelter.Infrastructure.Database;

namespace AnimalShelter.Tests.Controller;

[Collection("Factory")]
public class AccountControllerTests
{
    private readonly HttpClient _client;
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;
    public AccountControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _passwordHasher = new PasswordHasher<User>();
        _dbContext = factory.Services.GetRequiredService<ApplicationDbContext>();
    }

    [Fact]
    public async Task Register_Ok()
    {
        var command = new RegisterCommand("NewUser", "Password123!", "new@test.com");

        var response = await _client.PostAsJsonAsync(AccountRoutes.Register, command);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Register_UsernameTaken_BadRequest()
    {
        var command = new RegisterCommand(SeedUsers.UsernameEmp, "Password123!", "new@test.com");

        var response = await _client.PostAsJsonAsync(AccountRoutes.Register, command);

        var message = await response.Content.ReadFromJsonAsync<string>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("User with this username already exists.", message);
    }

    [Fact]
    public async Task Register_EmailTaken_BadRequest()
    {
        var command = new RegisterCommand("NewUser", "Password123!", SeedUsers.EmailEmp);

        var response = await _client.PostAsJsonAsync(AccountRoutes.Register, command);

        var message = await response.Content.ReadFromJsonAsync<string>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("User with this email already exists.", message);
    }

    [Fact]
    public async Task Login_Ok()
    {
        var command = new LoginCommand(SeedUsers.UsernameEmp, SeedUsers.PasswordEmp);

        var response = await _client.PostAsJsonAsync(AccountRoutes.Login, command);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Login_UsernameNotExist_BadRequest()
    {
        var command = new LoginCommand("WrongUser", SeedUsers.PasswordEmp);

        var response = await _client.PostAsJsonAsync(AccountRoutes.Login, command);
        var message = await response.Content.ReadFromJsonAsync<string>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("The user with the given username does not exist.", message);
    }

    [Fact]
    public async Task Login_WrongPassword_Ok()
    {
        var command = new LoginCommand(SeedUsers.UsernameEmp, "WrongPassword");

        var response = await _client.PostAsJsonAsync(AccountRoutes.Login, command);
        var message = await response.Content.ReadFromJsonAsync<string>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Incorrect password.", message);

    }
}
