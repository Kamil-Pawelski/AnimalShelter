using AnimalShelter.Domain;
using AnimalShelter.Domain.Repositores;
using AnimalShelter.Domain.UserEntities;
using Microsoft.EntityFrameworkCore;

namespace AnimalShelter.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AccountRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddDefaultRole(UserRole userRole)
    {
        _dbContext.UserRoles.Add(userRole);

        await _dbContext.SaveChangesAsync();
    }

    public async Task AddUser(User user)
    {
        _dbContext.Users.Add(user);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<Role?> GetRole(string name)
    {
        return await _dbContext.Roles.FirstOrDefaultAsync(role => role.Name == name);
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(user => user.Email == email);
    }
}
