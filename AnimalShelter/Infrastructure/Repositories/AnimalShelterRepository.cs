using AnimalShelter.App.DTO;
using AnimalShelter.Domain;
using AnimalShelter.Domain.AnimalShelterEntities;
using AnimalShelter.Domain.Repositores;
using Microsoft.EntityFrameworkCore;

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

    public async Task AdoptAnimal(AnimalAdopted animalAdopted)
    {
        _context.AnimalAdoptions.Add(animalAdopted);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAnimal(Animal animal)
    {
        _context.Animals.Remove(animal);
        await _context.SaveChangesAsync();
    }

    public async Task<List<AdoptedAnimalDTO>> GetAdoptedAnimals()
    {
        return await _context.Animals
            .Join(_context.AnimalAdoptions,
                  animal => animal.Id,
                  adoption => adoption.AnimalId,
                  (animal, adoption) => new AdoptedAnimalDTO(
                      animal.Id,
                      animal.Name,
                      animal.Species,
                      adoption.UserId.ToString(),
                      adoption.AdoptionDate
                  ))
            .ToListAsync();
    }

    public async Task<List<Animal>> GetAllAnimals()
    {
        return await _context.Animals.ToListAsync();
    }

    public async Task<List<Animal>> GetAllAnimalsByStatus(AdoptionStatus adoptionStatus)
    {
        return await _context.Animals
            .Where(animal => animal.AdoptionStatus == adoptionStatus)
            .ToListAsync();
    }

    public async Task<Animal?> GetAnimalById(int id)
    {
        return await _context.Animals.FindAsync(id);
    }

    public async Task<Animal> UpdateAnimal(Animal animal)
    {
        _context.Animals.Update(animal);
        await _context.SaveChangesAsync();
        return animal;
    }
}
