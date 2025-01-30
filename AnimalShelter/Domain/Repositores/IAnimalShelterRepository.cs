﻿using AnimalShelter.Domain.AnimalShelterEntities;

namespace AnimalShelter.Domain.Repositores;

public interface IAnimalShelterRepository
{
    Task AddAnimal(Animal animal);
    List<Animal> GetAllAnimalsByStatus(AdoptionStatus adoptionStatus);
    List<Animal> GetAllAnimals();
    Animal GetAnimalById(int id);
    Task<Animal> UpdateAnimal(Animal animal);
}
