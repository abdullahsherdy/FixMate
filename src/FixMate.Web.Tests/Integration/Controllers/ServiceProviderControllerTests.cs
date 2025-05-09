using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FixMate.Domain.Entities;
using FixMate.Domain.ValueObjects;
using FixMate.Domain.Enums;
using FixMate.Web.Tests.Integration;
using Xunit;

namespace FixMate.Web.Tests.Integration.Controllers
{
    public class ServiceProviderControllerTests : TestBase
    {
        private const string TestEmail = "test@example.com";
        private const string TestPassword = "Test123!";
        private Guid _userId;
        private Guid _serviceProviderId;
        private string _authToken;

        protected override void SeedTestData()
        {
            // Create test user
            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = "Test User",
                Email = new Email(TestEmail),
                PhoneNumber = "+1234567890",
                PasswordHash = TestPassword // In a real app, this would be hashed
            };
            _userId = user.Id;
            _context.Users.Add(user);

            // Create test service provider
            var serviceProvider = new ServiceProvider
            {
                Id = Guid.NewGuid(),
                FullName = "Test Provider",
                Email = "provider@example.com",
                PhoneNumber = "+1234567890",
                Specialization = Specialization.General,
                BusinessName = "Test Auto Service",
                BusinessAddress = "123 Main St",
                BusinessPhone = "+1234567890",
                BusinessEmail = "business@example.com",
                BusinessLicense = "LIC123",
                InsurancePolicy = "INS456",
                IsAvailable = true
            };
            _serviceProviderId = serviceProvider.Id;
            _context.ServiceProviders.Add(serviceProvider);

            _context.SaveChanges();
        }

        public ServiceProviderControllerTests()
        {
            _authToken = GetAuthTokenAsync(TestEmail, TestPassword).GetAwaiter().GetResult();
            SetAuthToken(_authToken);
        }

        [Fact]
        public async Task CreateServiceProvider_WithValidData_ReturnsCreated()
        {
            // Arrange
            var provider = new
            {
                FullName = "New Provider",
                Email = "newprovider@example.com",
                PhoneNumber = "+1987654321",
                Specialization = Specialization.Electrical,
                BusinessName = "New Auto Service",
                BusinessAddress = "456 Oak St",
                BusinessPhone = "+1987654321",
                BusinessEmail = "newbusiness@example.com",
                BusinessLicense = "LIC456",
                InsurancePolicy = "INS789"
            };

            // Act
            var response = await _client.PostAsync("/api/serviceproviders",
                new StringContent(JsonSerializer.Serialize(provider), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(content);
            Assert.NotNull(result.GetProperty("id").GetString());
        }

        [Fact]
        public async Task CreateServiceProvider_WithExistingEmail_ReturnsBadRequest()
        {
            // Arrange
            var provider = new
            {
                FullName = "New Provider",
                Email = "provider@example.com", // Using existing email
                PhoneNumber = "+1987654321",
                Specialization = Specialization.Electrical,
                BusinessName = "New Auto Service",
                BusinessAddress = "456 Oak St",
                BusinessPhone = "+1987654321",
                BusinessEmail = "newbusiness@example.com",
                BusinessLicense = "LIC456",
                InsurancePolicy = "INS789"
            };

            // Act
            var response = await _client.PostAsync("/api/serviceproviders",
                new StringContent(JsonSerializer.Serialize(provider), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetServiceProvider_WithValidId_ReturnsProvider()
        {
            // Arrange
            var provider = await _context.ServiceProviders.FindAsync(_serviceProviderId);

            // Act
            var response = await _client.GetAsync($"/api/serviceproviders/{provider.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(content);
            Assert.Equal(provider.Id.ToString(), result.GetProperty("id").GetString());
            Assert.Equal(provider.FullName, result.GetProperty("fullName").GetString());
        }

        [Fact]
        public async Task GetServiceProvider_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync($"/api/serviceproviders/{Guid.NewGuid()}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetServiceProvidersBySpecialization_ReturnsProviders()
        {
            // Act
            var response = await _client.GetAsync($"/api/serviceproviders/specialization/{Specialization.General}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(content);
            Assert.True(result.GetArrayLength() > 0);
        }

        [Fact]
        public async Task UpdateServiceProvider_WithValidData_ReturnsOk()
        {
            // Arrange
            var provider = await _context.ServiceProviders.FindAsync(_serviceProviderId);
            var updateData = new
            {
                FullName = "Updated Provider",
                Email = provider.Email,
                PhoneNumber = provider.PhoneNumber,
                Specialization = provider.Specialization,
                BusinessName = "Updated Auto Service",
                BusinessAddress = provider.BusinessAddress,
                BusinessPhone = provider.BusinessPhone,
                BusinessEmail = provider.BusinessEmail,
                BusinessLicense = provider.BusinessLicense,
                InsurancePolicy = provider.InsurancePolicy
            };

            // Act
            var response = await _client.PutAsync($"/api/serviceproviders/{provider.Id}",
                new StringContent(JsonSerializer.Serialize(updateData), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(content);
            Assert.Equal(updateData.FullName, result.GetProperty("fullName").GetString());
            Assert.Equal(updateData.BusinessName, result.GetProperty("businessName").GetString());
        }

        [Fact]
        public async Task UpdateAvailability_WithValidData_ReturnsNoContent()
        {
            // Arrange
            var provider = await _context.ServiceProviders.FindAsync(_serviceProviderId);
            var updateData = new
            {
                IsAvailable = false
            };

            // Act
            var response = await _client.PutAsync($"/api/serviceproviders/{provider.Id}/availability",
                new StringContent(JsonSerializer.Serialize(updateData), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task GetAssignedRequests_ReturnsRequests()
        {
            // Arrange
            var provider = await _context.ServiceProviders.FindAsync(_serviceProviderId);

            // Act
            var response = await _client.GetAsync($"/api/serviceproviders/{provider.Id}/requests");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(content);
            Assert.NotNull(result);
        }
    }
} 