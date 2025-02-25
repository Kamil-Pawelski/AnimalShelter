using AnimalShelter.Domain.AnimalShelterEntities;
using AnimalShelter.Domain.Constants;
using AnimalShelter.Domain.UserEntities;
using AnimalShelter.Infrastructure.Database;
using AnimalShelter.Tests.Controller;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AnimalShelter.Tests;

public class CustomWebApplicationFactory<TProgram>
: WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddJsonFile("appsettings.Test.json");
            config.AddUserSecrets<AnimalShelterControllerTests>();
            config.AddUserSecrets<AccountControllerTests>();
        });

        builder.ConfigureServices((context, services) =>
        {
            var connectionString = context.Configuration.GetConnectionString("TestConnection");
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<ApplicationDbContext>));

            if (dbContextDescriptor != null)
                services.Remove(dbContextDescriptor);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureDeleted();
                db.Database.Migrate();

                var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();
                SeedTestData(db, passwordHasher);
            }
        });

        builder.UseEnvironment("Test");
    }
    private void SeedTestData(ApplicationDbContext dbContext, IPasswordHasher<User> passwordHasher)
    {
        var users = new List<User>
        {
            new()
            {
                Username = SeedUsers.UsernameEmp,
                Email = SeedUsers.EmailEmp,
                Password = passwordHasher.HashPassword(null!, SeedUsers.PasswordEmp)
            },
            new() 
            {
                Username = SeedUsers.UsernameUser,
                Email = SeedUsers.EmailUser,
                Password = passwordHasher.HashPassword(null!, SeedUsers.PasswordUser)
            }
        };

        dbContext.Users.AddRange(users);

        dbContext.SaveChanges();

        var roles = dbContext.Roles.Where(r => r.Name == RolesConstants.Employee || r.Name == RolesConstants.User).ToDictionary(r => r.Name, r => r.Id);

        var userRoles = new List<UserRole>
        {
            new() { UserId = users[0].Id, RoleId = roles[RolesConstants.Employee] },
            new() { UserId = users[1].Id, RoleId = roles[RolesConstants.User] }
        };

        dbContext.UserRoles.AddRange(userRoles);

        var animals = new List<Animal>
        {
            new()
            {
                Name = SeedAnimal.NameAdopted,
                Species = SeedAnimal.SpeciesAdopted,
                Breed = SeedAnimal.BreedAdopted,
                Age = SeedAnimal.AgeAdopted,
                Weight = SeedAnimal.WeightAdopted,
                AdoptionStatus = AdoptionStatus.Adopted
            },
            new()
            {
                Name = SeedAnimal.NameAvailable,
                Species = SeedAnimal.SpeciesAvailable,
                Breed = SeedAnimal.BreedAvailable,
                Age = SeedAnimal.AgeAvailable,
                Weight = SeedAnimal.WeightAvailable,
                AdoptionStatus = AdoptionStatus.Available
            }
        };
        dbContext.Animals.AddRange(animals);

        dbContext.SaveChanges();

        var animalAdopted = new AnimalAdopted { AdoptionDate = DateTime.Now, AnimalId = animals[0].Id, UserId = users[1].Id };
        dbContext.AnimalAdoptions.Add(animalAdopted);

        dbContext.SaveChanges();
    }
}

[CollectionDefinition("Factory")]
public class SharedCustomWebApplicationFactory : IClassFixture<CustomWebApplicationFactory<Program>>
{
}
