namespace AnimalShelter.App.DTO;

public class AdoptedAnimalDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Species { get; set; }
    public string AdoptedBy { get; set; }
    public DateTime AdoptedDate { get; set; }
    public AdoptedAnimalDTO(int id, string name, string species, int adoptedBy, DateTime adoptedDate)
    {
        Id = id;
        Name = name;
        Species = species;
        AdoptedBy = $"User {adoptedBy}";
        AdoptedDate = adoptedDate;
    }
}
