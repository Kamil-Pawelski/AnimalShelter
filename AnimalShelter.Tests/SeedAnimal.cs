using AnimalShelter.Domain.AnimalShelterEntities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimalShelter.Tests;

public static class SeedAnimal
{
    public const string NameAdopted = "SeedAdoptedAnimal";
    public const string SpeciesAdopted = "SeedAdoptedSpecies";
    public const string BreedAdopted = "SeedAdoptedBreed";
    public const int AgeAdopted = 2;
    public const int WeightAdopted = 20;
    public const AdoptionStatus AdoptionStatusAdopted = AdoptionStatus.Adopted;

    public const string NameAvailable = "SeedAvailableAnimal";
    public const string SpeciesAvailable = "SeedAvailableSpecies";
    public const string BreedAvailable = "SeedAvailableBreed";
    public const int AgeAvailable = 3;
    public const int WeightAvailable = 25;
    public const AdoptionStatus AdoptionStatusAvailable = AdoptionStatus.Available;

}
