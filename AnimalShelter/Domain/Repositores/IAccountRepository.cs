using AnimalShelter.Domain.UserEntities;

namespace AnimalShelter.Domain.Repositores;

public interface IAccountRepository
{
    Task AddUser(User user);
    Task<User?> GetUserByEmail(string email);
    Task AddDefaultRole(UserRole userRole);
    Task<Role?> GetRole(string name);
}
