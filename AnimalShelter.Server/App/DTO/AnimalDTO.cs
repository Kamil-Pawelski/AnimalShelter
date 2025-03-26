using AnimalShelter.Domain.AnimalShelterEntities;

namespace AnimalShelter.App.DTO;

public class AnimalDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Species { get; set; }
    public string Breed { get; set; }
    public int Age { get; set; }
    public int Weight { get; set; }
    public AdoptionStatus AdoptionStatus { get; set; }

    public AnimalDTO(Animal animal)
    {
        Id = animal.Id;
        Name = animal.Name;
        Species = animal.Species;
        Breed = animal.Breed;
        Age = animal.Age;
        Weight = animal.Weight;
        AdoptionStatus = animal.AdoptionStatus;
    }
}
