using System;
using System.Threading.Tasks;
using Fixmate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BC = BCrypt.Net.BCrypt;

namespace FixMate.Infrastructure.Persistence.Seeders
{
    public class DatabaseSeeder
    {
        private readonly FixMateDbContext _context;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(FixMateDbContext context, ILogger<DatabaseSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                // Seed roles if they don't exist
                if (!await _context.Roles.AnyAsync())
                {
                    var adminRole = new Role
                    {
                        Id = Guid.Parse("1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p"),
                        Name = "Admin",
                        Description = "Administrator with full access"
                    };

                    var userRole = new Role
                    {
                        Id = Guid.Parse("2b3c4d5e-6f7g-8h9i-0j1k-2l3m4n5o6p7q"),
                        Name = "User",
                        Description = "Regular user with limited access"
                    };

                    await _context.Roles.AddRangeAsync(adminRole, userRole);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Roles seeded successfully");
                }

                // Create admin user if it doesn't exist
                if (!await _context.Users.AnyAsync(u => u.Email == "admin@fixmate.com"))
                {
                    var adminUser = new User
                    {
                        Id = Guid.NewGuid(),
                        Email = "admin@fixmate.com",
                        PasswordHash = BC.HashPassword("Admin123!"), // Change this password
                        FirstName = "Admin",
                        LastName = "User",
                        PhoneNumber = "1234567890",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };

                    await _context.Users.AddAsync(adminUser);
                    await _context.SaveChangesAsync();

                    // Assign admin role to the user
                    var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
                    if (adminRole != null)
                    {
                        var userRole = new UserRole
                        {
                            UserId = adminUser.Id,
                            RoleId = adminRole.Id
                        };

                        await _context.UserRoles.AddAsync(userRole);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Admin user created successfully");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database");
                throw;
            }
        }
    }
} 