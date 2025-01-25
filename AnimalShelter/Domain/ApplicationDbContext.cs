using AnimalShelter.Domain.AnimalShelterEntities;
using AnimalShelter.Domain.UserEntities;
using Microsoft.EntityFrameworkCore;

namespace AnimalShelter.Domain;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<UserRole>()
            .HasOne(o => o.User)
            .WithMany(m => m.UserRoles)
            .HasForeignKey(fk => fk.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserRole>()
            .HasOne(o => o.Role)
            .WithMany(m => m.UserRoles)
            .HasForeignKey(fk => fk.RoleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AnimalAdopted>()
            .HasKey(aa => new {aa.AnimalId, aa.UserId});

        modelBuilder.Entity<AnimalAdopted>()
            .HasOne(o => o.User)
            .WithMany(m => m.AnimalAdoptions)
            .HasForeignKey(fk => fk.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AnimalAdopted>()
            .HasOne(o => o.Animal)
            .WithMany(m => m.AnimalAdoptions)
            .HasForeignKey(fk => fk.AnimalId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        //Enum handler
        modelBuilder.Entity<Animal>()
            .Property(a => a.AdoptionStatus)
            .HasConversion<string>();
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Animal> Animals { get; set; }
    public DbSet<AnimalAdopted> AnimalAdoptions { get; set; }

}
