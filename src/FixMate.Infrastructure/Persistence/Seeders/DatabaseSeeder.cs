using System;
using System.Threading.Tasks;
using FixMate.Domain.Entities;
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
                // Create admin user if it doesn't exist
                if (!await _context.Users.AnyAsync(u => u.Email.ToString() == "admin@fixmate.com"))
                {
                    var adminUser = new User
                    {
                        Id = Guid.NewGuid(),
                        Email = "admin@fixmate.com",
                        PasswordHash = BC.HashPassword("Admin@123!"), // Change this password
                        FullName = "Admin User",
                        Role = Role.Admin,
                        PhoneNumber = "1234567890",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };

                    await _context.Users.AddAsync(adminUser);
                    await _context.SaveChangesAsync();
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