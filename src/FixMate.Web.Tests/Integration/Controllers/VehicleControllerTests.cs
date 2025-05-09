using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FixMate.Domain.Entities;
using FixMate.Domain.ValueObjects;
using FixMate.Web.Tests.Integration;
using Xunit;

namespace FixMate.Web.Tests.Integration.Controllers
{
    public class VehicleControllerTests : TestBase
    {
        private const string TestEmail = "test@example.com";
        private const string TestPassword = "Test123!";
        private Guid _userId;
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

            // Create test vehicle
            var vehicle = new Vehicle
            {
                Id = Guid.NewGuid(),
                Make = "Toyota",
                Model = "Camry",
                Year = 2020,
                LicensePlate = "ABC123",
                VIN = "1HGCM82633A123456",
                OwnerId = user.Id
            };
            _context.Vehicles.Add(vehicle);

            _context.SaveChanges();
        }

        public VehicleControllerTests()
        {
            _authToken = GetAuthTokenAsync(TestEmail, TestPassword).GetAwaiter().GetResult();
            SetAuthToken(_authToken);
        }

        [Fact]
        public async Task CreateVehicle_WithValidData_ReturnsCreated()
        {
            // Arrange
            var vehicle = new
            {
                Make = "Honda",
                Model = "Accord",
                Year = 2021,
                LicensePlate = "XYZ789",
                VIN = "2HGCM82633A123456",
                OwnerId = _userId
            };

            // Act
            var response = await _client.PostAsync("/api/vehicles",
                new StringContent(JsonSerializer.Serialize(vehicle), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(content);
            Assert.NotNull(result.GetProperty("id").GetString());
        }

        [Fact]
        public async Task CreateVehicle_WithDuplicateVIN_ReturnsBadRequest()
        {
            // Arrange
            var vehicle = new
            {
                Make = "Honda",
                Model = "Accord",
                Year = 2021,
                LicensePlate = "XYZ789",
                VIN = "1HGCM82633A123456", // Using existing VIN
                OwnerId = _userId
            };

            // Act
            var response = await _client.PostAsync("/api/vehicles",
                new StringContent(JsonSerializer.Serialize(vehicle), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetVehicle_WithValidId_ReturnsVehicle()
        {
            // Arrange
            var vehicle = await _context.Vehicles.FirstAsync();

            // Act
            var response = await _client.GetAsync($"/api/vehicles/{vehicle.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(content);
            Assert.Equal(vehicle.Id.ToString(), result.GetProperty("id").GetString());
            Assert.Equal(vehicle.Make, result.GetProperty("make").GetString());
        }

        [Fact]
        public async Task GetVehicle_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync($"/api/vehicles/{Guid.NewGuid()}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetVehiclesByOwner_ReturnsVehicles()
        {
            // Act
            var response = await _client.GetAsync($"/api/vehicles/owner/{_userId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(content);
            Assert.True(result.GetArrayLength() > 0);
        }

        [Fact]
        public async Task UpdateVehicle_WithValidData_ReturnsOk()
        {
            // Arrange
            var vehicle = await _context.Vehicles.FirstAsync();
            var updateData = new
            {
                Make = "Updated Make",
                Model = "Updated Model",
                Year = 2022,
                LicensePlate = vehicle.LicensePlate,
                VIN = vehicle.VIN,
                OwnerId = vehicle.OwnerId
            };

            // Act
            var response = await _client.PutAsync($"/api/vehicles/{vehicle.Id}",
                new StringContent(JsonSerializer.Serialize(updateData), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(content);
            Assert.Equal(updateData.Make, result.GetProperty("make").GetString());
            Assert.Equal(updateData.Model, result.GetProperty("model").GetString());
        }

        [Fact]
        public async Task DeleteVehicle_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var vehicle = await _context.Vehicles.FirstAsync();

            // Act
            var response = await _client.DeleteAsync($"/api/vehicles/{vehicle.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeleteVehicle_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var response = await _client.DeleteAsync($"/api/vehicles/{Guid.NewGuid()}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
} 