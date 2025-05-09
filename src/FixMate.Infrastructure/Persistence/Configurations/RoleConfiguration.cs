using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Fixmate.Domain.Entities;

namespace FixMate.Infrastructure.Persistence.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(r => r.Description)
                .HasMaxLength(200);

            // Seed default roles
            builder.HasData(
                new Role
                {
                    Id = Guid.Parse("1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p"),
                    Name = "Admin",
                    Description = "Administrator with full access"
                },
                new Role
                {
                    Id = Guid.Parse("2b3c4d5e-6f7g-8h9i-0j1k-2l3m4n5o6p7q"),
                    Name = "User",
                    Description = "Regular user with limited access"
                }
            );
        }
    }
} 