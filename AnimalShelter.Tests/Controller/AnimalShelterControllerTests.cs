using System.Net.Http.Json;
using System.Net;
using AnimalShelter.App.Routes;
using AnimalShelter.App.Commands;
using System.Net.Http.Headers;
using AnimalShelter.App.Queries;
using AnimalShelter.Domain.AnimalShelterEntities;


namespace AnimalShelter.Tests.Controller;

[Collection("Factory")]
public class AnimalShelterControllerTests
{
    private readonly HttpClient _client;
    public AnimalShelterControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostAnimal_EmployeeRole_ReturnsOk()
    {
        // Move to signle function dry
        var loginCommand = new LoginCommand(SeedUsers.UsernameEmp, SeedUsers.PasswordEmp);
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

    [Fact]
    public async Task PostAnimal_UserRole_ReturnsForbidden()
    {
        var loginCommand = new LoginCommand(SeedUsers.UsernameUser, SeedUsers.PasswordUser);
        var loginResponse = await _client.PostAsJsonAsync(AccountRoutes.Login, loginCommand);

        var token = await loginResponse.Content.ReadFromJsonAsync<string>();

        var animal = new PostAnimalCommand("Buddy", "Dog", "Golden Retriever", 3, 30);

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, AnimalShelterRoutes.PostAnimal)
        {
            Content = JsonContent.Create(animal)
        };

        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _client.SendAsync(requestMessage);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task PostAnimal_NotLogged_ReturnsUnauthorized()
    {
      
        var animal = new PostAnimalCommand("Buddy", "Dog", "Golden Retriever", 3, 30);

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, AnimalShelterRoutes.PostAnimal)
        {
            Content = JsonContent.Create(animal)
        };

        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", null);
        var response = await _client.SendAsync(requestMessage);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAnimal_ReturnsOk()
    {
        var loginCommand = new LoginCommand(SeedUsers.UsernameUser, SeedUsers.PasswordUser);
        var loginResponse = await _client.PostAsJsonAsync(AccountRoutes.Login, loginCommand);

        var token = await loginResponse.Content.ReadFromJsonAsync<string>();
       
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/animals/1");   

        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(requestMessage);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAnimal_ReturnsBadRequest()
    {
        var loginCommand = new LoginCommand(SeedUsers.UsernameUser, SeedUsers.PasswordUser);
        var loginResponse = await _client.PostAsJsonAsync(AccountRoutes.Login, loginCommand);

        var token = await loginResponse.Content.ReadFromJsonAsync<string>();

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/animals/1000");

        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(requestMessage);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
