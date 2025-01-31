using AnimalShelter.App.DTO;
using AnimalShelter.Domain.AnimalShelterEntities;

namespace AnimalShelter.Domain.Repositores;

public interface IAnimalShelterRepository
{
    Task AddAnimal(Animal animal);
    Task<List<Animal>> GetAllAnimalsByStatus(AdoptionStatus adoptionStatus);
    Task<List<Animal>> GetAllAnimals();
    Task<Animal?> GetAnimalById(int id);
    Task<Animal> UpdateAnimal(Animal animal);
    Task DeleteAnimal(Animal animal);
    Task AdoptAnimal(AnimalAdopted animalAdopted);
    Task<List<AdoptedAnimalDTO>> GetAdoptedAnimals();
}

