using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using FixMate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using FixMate.Web;

namespace FixMate.Web.Tests.Integration
{
    public abstract class TestBase : IDisposable
    {
        protected readonly WebApplicationFactory<Program> _factory;
        protected readonly HttpClient _client;
        protected readonly FixMateDbContext _context;

        protected TestBase()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        // Remove the existing DbContext registration
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(DbContextOptions<FixMateDbContext>));

                        if (descriptor != null)
                        {
                            services.Remove(descriptor);
                        }

                        // Add in-memory database
                        services.AddDbContext<FixMateDbContext>(options =>
                        {
                            options.UseInMemoryDatabase(Guid.NewGuid().ToString());
                        });

                        // Build the service provider
                        var sp = services.BuildServiceProvider();

                        // Create a scope to obtain a reference to the database context
                        using (var scope = sp.CreateScope())
                        {
                            var scopedServices = scope.ServiceProvider;
                            _context = scopedServices.GetRequiredService<FixMateDbContext>();

                            // Ensure the database is created
                            _context.Database.EnsureCreated();

                            try
                            {
                                // Seed the database with test data
                                SeedTestData();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("An error occurred seeding the database. Error: {0}", ex.Message);
                            }
                        }
                    });
                });

            _client = _factory.CreateClient();
        }

        protected virtual void SeedTestData()
        {
            // Override in derived classes to seed test data
        }

        protected async Task<string> GetAuthTokenAsync(string email, string password)
        {
            var loginRequest = new
            {
                Email = email,
                Password = password
            };

            var response = await _client.PostAsync("/api/auth/login",
                new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(content);
                return result.GetProperty("token").GetString();
            }

            return null;
        }

        protected void SetAuthToken(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            _client.Dispose();
            _factory.Dispose();
        }
    }
} 