using System.Net.Http.Json;
using System.Net;
using AnimalShelter.App.Routes;
using AnimalShelter.App.Commands;
using System.Net.Http.Headers;
using AnimalShelter.App.DTO;
using AnimalShelter.Domain.AnimalShelterEntities;
using Azure.Core;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Azure;

namespace AnimalShelter.Tests.Controller;

[Collection("Factory")]
public class AnimalShelterControllerTests
{
    private readonly HttpClient _client;
    private readonly string _employeeToken;
    private readonly string _userToken;

    public AnimalShelterControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _employeeToken = AuthenticateAsync(SeedUsers.UsernameEmp, SeedUsers.PasswordEmp).Result;
        _userToken = AuthenticateAsync(SeedUsers.UsernameUser, SeedUsers.PasswordUser).Result;
    }

    private async Task<string> AuthenticateAsync(string username, string password)
    {
        var loginCommand = new LoginCommand(username, password);
        var loginResponse = await _client.PostAsJsonAsync(AccountRoutes.Login, loginCommand);

        return await loginResponse.Content.ReadFromJsonAsync<string>() ?? string.Empty;
    }

    [Fact]
    public async Task PostAnimal_EmployeeRole_ReturnsOk()
    {
        var animal = new PostAnimalCommand("Buddy", "Dog", "Golden Retriever", 3, 30);
        var response = await SendAuthorizedPostRequest(AnimalShelterRoutes.PostAnimal, _employeeToken, animal);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PostAnimal_UserRole_ReturnsForbidden()
    {
        var animal = new PostAnimalCommand("Buddy", "Dog", "Golden Retriever", 3, 30);
        var response = await SendAuthorizedPostRequest(AnimalShelterRoutes.PostAnimal, _userToken, animal);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task PostAnimal_NotLogged_ReturnsUnauthorized()
    {
        var animal = new PostAnimalCommand("Buddy", "Dog", "Golden Retriever", 3, 30);
        var response = await SendAuthorizedPostRequest(AnimalShelterRoutes.PostAnimal, null, animal);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAnimal_ReturnsOk()
    {
        var response = await SendAuthorizedGetRequest("/animals/1", _userToken);
        var result = await response.Content.ReadFromJsonAsync<Animal>();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetAnimal_ReturnsNotFound()
    {
        var response = await SendAuthorizedGetRequest("/animals/1000", _userToken);
        var result = await response.Content.ReadFromJsonAsync<string>();
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("The animal with the given ID does not exist.", result);
    }

    [Fact]
    public async Task GetAnimals_EmployeeRole_ReturnsOk()
    {
        var response = await SendAuthorizedGetRequest(AnimalShelterRoutes.GetAnimals, _employeeToken);
        var result = await response.Content.ReadFromJsonAsync<List<Animal>>();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetAnimals_UserRole_ReturnsOk()
    {
        var response = await SendAuthorizedGetRequest(AnimalShelterRoutes.GetAnimals, _userToken);
        var result = await response.Content.ReadFromJsonAsync<List<Animal>>();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.DoesNotContain(result!, animal => animal.AdoptionStatus == AdoptionStatus.Adopted);

    }

    [Fact]
    public async Task GetAnimals_NotLogged_ReturnsUnauthorized()
    {
        var response = await SendAuthorizedGetRequest(AnimalShelterRoutes.GetAnimals, "");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PutAnimals_EmployeeRole_ReturnsOk()
    {
        var updatedAnimal = new PutAnimalCommand("Heniek", "Dog", "Golden Retriever", 3, 30, AdoptionStatus.Available);
        var response = await SendAuthorizedPutRequest("/animals/1", _employeeToken, updatedAnimal);
        var result = await response.Content.ReadFromJsonAsync<Animal>();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.NotEqual(SeedAnimal.NameAdopted, result.Name);
    }

    [Fact]
    public async Task PutAnimals_EmployeeRole_ReturnsNotFound()
    {
        var updatedAnimal = new PutAnimalCommand("Heniek", "Dog", "Golden Retriever", 3, 30, AdoptionStatus.Available);
        var response = await SendAuthorizedPutRequest("/animals/1000", _employeeToken, updatedAnimal);
        var result = await response.Content.ReadFromJsonAsync<string>();
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(result);
        Assert.Equal("The animal with the given ID does not exist.", result);
    }

    [Fact]
    public async Task PutAnimals_UserRole_ReturnsForbidden()
    {
        var updatedAnimal = new PutAnimalCommand("Heniek", "Dog", "Golden Retriever", 3, 30, AdoptionStatus.Available);
        var response = await SendAuthorizedPutRequest("/animals/1", _userToken, updatedAnimal);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
    [Fact]
    public async Task PutAnimals_UserRole_ReturnsUnauthorized()
    {
        var updatedAnimal = new PutAnimalCommand("Heniek", "Dog", "Golden Retriever", 3, 30, AdoptionStatus.Available);
        var response = await SendAuthorizedPutRequest("/animals/1", "", updatedAnimal);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAnimal_EmployeeRole_ReturnsOk()
    {
        var newAnimal = new PostAnimalCommand("Rocky", "Dog", "Bulldog", 5, 25);
        var postResponse = await SendAuthorizedPostRequest(AnimalShelterRoutes.PostAnimal, _employeeToken, newAnimal);
        postResponse.EnsureSuccessStatusCode();

        var createdAnimal = await postResponse.Content.ReadFromJsonAsync<Animal>();
        Assert.NotNull(createdAnimal);

        var deleteResponse = await SendAuthorizedDeleteRequest($"/animals/{createdAnimal.Id}", _employeeToken);
        var result = await deleteResponse.Content.ReadFromJsonAsync<string>();

        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        Assert.Equal($"Animal with ID {createdAnimal.Id} has been removed from the shelter.", result);
    }

    [Fact]
    public async Task DeleteAnimal_EmployeeRole_ReturnsNotFound()
    {
        var newAnimal = new PostAnimalCommand("Rocky", "Dog", "Bulldog", 5, 25);
        var postResponse = await SendAuthorizedPostRequest(AnimalShelterRoutes.PostAnimal, _employeeToken, newAnimal);
        postResponse.EnsureSuccessStatusCode();

        var createdAnimal = await postResponse.Content.ReadFromJsonAsync<Animal>();
        Assert.NotNull(createdAnimal);

        var deleteResponse = await SendAuthorizedDeleteRequest("/animals/1000", _employeeToken);
        var result = await deleteResponse.Content.ReadFromJsonAsync<string>();

        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
        Assert.Equal("The animal with the given ID does not exist.", result);
    }

    [Fact]
    public async Task DeleteAnimal_UserRole_ReturnsForbidden()
    {
        var response = await SendAuthorizedDeleteRequest("/animals/1000", _userToken);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }


    [Fact]
    public async Task DeleteAnimal_NotLogged_ReturnsUnathorized()
    {    
        var response = await SendAuthorizedDeleteRequest("/animals/1000", "");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAdoptedAnimal_EmployeeRole_Ok()
    {
        var response = await SendAuthorizedGetRequest(AnimalShelterRoutes.GetAdoptedAnimals, _employeeToken);
        var result = await response.Content.ReadFromJsonAsync<List<AdoptedAnimalDTO>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetAdoptedAnimal_UserRole_Forbidden()
    {
        var response = await SendAuthorizedGetRequest(AnimalShelterRoutes.GetAdoptedAnimals, _userToken);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetAdoptedAnimal_NotLogged_Unathorized()
    {
        var response = await SendAuthorizedGetRequest(AnimalShelterRoutes.GetAdoptedAnimals, "");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PostAdoptAnimal_Ok()
    {
        var adoptAnimal = new PostAdoptAnimalCommand(1, 2);
        var response = await SendAuthorizedPostRequest(AnimalShelterRoutes.PostAdoptAnimal, _userToken, adoptAnimal);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PostAdoptAnimal_Unathorized()
    {
        var adoptAnimal = new PostAdoptAnimalCommand(1, 2);
        var response = await SendAuthorizedPostRequest(AnimalShelterRoutes.PostAdoptAnimal, "", adoptAnimal);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task<HttpResponseMessage> SendAuthorizedPostRequest<T>(string url, string? token, T content)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(content)
        };

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await _client.SendAsync(request);
    }

    private async Task<HttpResponseMessage> SendAuthorizedDeleteRequest(string url, string? token)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, url);

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await _client.SendAsync(request);
    }

    private async Task<HttpResponseMessage> SendAuthorizedPutRequest<T>(string url, string? token, T content)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, url)
        {
            Content = JsonContent.Create(content)
        };

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await _client.SendAsync(request);
    }

    private async Task<HttpResponseMessage> SendAuthorizedGetRequest(string url, string? token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await _client.SendAsync(request);
    }
}
