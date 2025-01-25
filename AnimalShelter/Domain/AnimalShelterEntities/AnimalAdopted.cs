using AnimalShelter.Domain.UserEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnimalShelter.Domain.AnimalShelterEntities;

[Table("AnimalAdoptions")]
public class AnimalAdopted
{
    [Required]
    [Key]
    public int AnimalId { get; set; }
    [Required]
    [Key]
    public int UserId { get; set; }
    [Required]
    public DateTime AdoptionDate { get; set; }

    public User? User { get; set; }
    public Animal? Animal { get; set; }
}
