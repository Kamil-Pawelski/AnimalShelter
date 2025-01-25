using AnimalShelter.Domain.AnimalShelterEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnimalShelter.Domain.UserEntities;

[Table("Users")]
public class User
{
    [Required]
    [Key]
    public int Id { get; set; }
    [Required]
    public string Username { get; set; } = null!;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;

    public ICollection<UserRole>? UserRoles { get; set; }
    public ICollection<AnimalAdopted>? AnimalAdoptions { get; set; }
}
