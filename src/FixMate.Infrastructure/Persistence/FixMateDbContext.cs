using Microsoft.EntityFrameworkCore;
using FixMate.Infrastructure.Persistence;
using FixMate.Domain.Entities;
namespace FixMate.Infrastructure.Persistence
{
    /// <summary>
    ///  EF Core 
    /// </summary>
    /// access way to deal with database in EF core 
    /// represetn database 
    public class FixMateDbContext : DbContext
    {
        public FixMateDbContext(DbContextOptions<FixMateDbContext> options) : base(options){}

        /// <summary>
        ///  Dbset -> Table 
        /// </summary>
        public DbSet<User> Users { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; } // Bike, Car, etc.. 
        public DbSet<ServiceRequest> ServiceRequests { get; set; } // Service 
        public DbSet<ServiceProvider> ServiceProviders { get; set; } /// Mechanics 


        /// Database Constraints in EF 
        /// DataAnnotations 
        /// FluentAPI
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        // Fluent Api syntax 
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id); //primary 

                entity.HasMany(e => e.Vehicles)
                      .WithOne(v => v.Owner)
                      .HasForeignKey(v => v.OwnerId);

                entity.HasMany(e => e.ServiceRequests)
                      .WithOne(sr => sr.Vehicle.Owner)
                      .HasForeignKey(sr => sr.VehicleId)
                      .OnDelete(DeleteBehavior.NoAction); // To avoid circular cascade delete, Cascade 
              
            });

            // Vehicle
            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasMany(v => v.ServiceRequests)
                      .WithOne(sr => sr.Vehicle)
                      .HasForeignKey(sr => sr.VehicleId);
            });

            // ServiceRequest
            modelBuilder.Entity<ServiceRequest>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(sr => sr.Vehicle)
                      .WithMany(v => v.ServiceRequests)
                      .HasForeignKey(sr => sr.VehicleId);

                entity.HasOne(sr => sr.AssignedProvider)
                      .WithMany(sp => sp.AssignedRequests)
                      .HasForeignKey(sr => sr.AssignedProviderId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // ServiceProvider
            modelBuilder.Entity<ServiceProvider>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasMany(sp => sp.AssignedRequests)
                      .WithOne(sr => sr.AssignedProvider)
                      .HasForeignKey(sr => sr.AssignedProviderId);
            });

            
        }


    }
} 