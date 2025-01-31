using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Net;
using AnimalShelter.App.Routes;
using AnimalShelter.App.Commands;
using System.Net.Http.Headers;
using AnimalShelter.Constants;
using AnimalShelter.Domain.UserEntities;
using Microsoft.EntityFrameworkCore;
using AnimalShelter.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using AnimalShelter.Domain.AnimalShelterEntities;
using Newtonsoft.Json.Linq;


namespace AnimalShelter.Tests.Controller;

public class AnimalShelterControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly ApplicationDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;
    public AnimalShelterControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _passwordHasher = new PasswordHasher<User>();
        _dbContext = factory.Services.GetRequiredService<ApplicationDbContext>();
    }
    private async Task SeedTestUser()
    {
        var user = new User
        {
            Username = "TestUser",
            Email = "testuser@test.com",            
        };

        user.Password = _passwordHasher.HashPassword(user, "Test123!");

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var role = _dbContext.Roles.First(role => role.Name == RolesConstants.Employee);

        _dbContext.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id });

        await _dbContext.SaveChangesAsync();

    }

    [Fact]
    public async Task Register_Ok()
    {
        var command = new RegisterCommand("NewUser", "Password123!", "new@test.com");

        var response = await _client.PostAsJsonAsync(AccountRoutes.Register, command);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PostAnimal_EmployeeRole_ReturnsOk()
    {
        await SeedTestUser();

        var loginCommand = new LoginCommand("TestUser", "Test123!");
        var loginResponse = await _client.PostAsJsonAsync(AccountRoutes.Login, loginCommand);

        var token = await loginResponse.Content.ReadFromJsonAsync<string>();

        var animal = new PostAnimalCommand("Buddy", "Dog", "Golden Retriever", 3, 30);

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, AnimalShelterRoutes.PostAnimal)
        {
            Content = JsonContent.Create(animal)
        };

        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.SendAsync(requestMessage);

        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

}
