using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnimalShelter.Domain.UserEntities;

[Table("UserRoles")]
public class UserRole
{
    [Required]
    [Key]
    public int UserId { get; set; }
    [Required]
    [Key]
    public int RoleId { get; set; }

    public User? User { get; set; }
    public Role? Role { get; set; }
}
