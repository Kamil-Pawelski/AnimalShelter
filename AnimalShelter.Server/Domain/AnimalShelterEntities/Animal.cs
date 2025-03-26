using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnimalShelter.Domain.AnimalShelterEntities;

[Table("Animals")]
public class Animal
{
    [Key]
    [Required]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = null!;
    [Required]
    public string Species { get; set; } = null!;
    [Required]
    public string Breed { get; set; } = null!;
    [Required]
    public int Age { get; set; }
    [Required]
    public int Weight { get; set; }
    [Required]
    public AdoptionStatus AdoptionStatus { get; set; }

    public ICollection<AnimalAdopted>? AnimalAdoptions { get; set; }
}
