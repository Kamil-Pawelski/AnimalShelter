using AnimalShelter.Domain;
using AnimalShelter.Domain.AnimalShelterEntities;
using AnimalShelter.Domain.Repositores;

namespace AnimalShelter.Infrastructure.Repositories;

public class AnimalShelterRepository : IAnimalShelterRepository
{
    private readonly ApplicationDbContext _context;

    public AnimalShelterRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAnimal(Animal animal)
    {
        _context.Animals.Add(animal);
        await _context.SaveChangesAsync();
    }

    public List<Animal> GetAllAnimals()
    {
        return _context.Animals.ToList();
    }

    public List<Animal> GetAllAnimalsByStatus(AdoptionStatus adoptionStatus)
    {
        return  _context.Animals.Where(animal => animal.AdoptionStatus == adoptionStatus).ToList();
    }
}
