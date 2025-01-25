    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace AnimalShelter.Domain.UserEntities;

    [Table("Roles")]
    public class Role
    {
        [Required]
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;

        public ICollection<UserRole>? UserRoles { get; set; }
    }
