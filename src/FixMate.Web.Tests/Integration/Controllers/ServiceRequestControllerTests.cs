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
    public class ServiceRequestControllerTests : TestBase
    {
        private const string TestEmail = "test@example.com";
        private const string TestPassword = "Test123!";
        private Guid _userId;
        private Guid _vehicleId;
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
            _vehicleId = vehicle.Id;
            _context.Vehicles.Add(vehicle);

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

            // Create test service request
            var serviceRequest = new ServiceRequest
            {
                Id = Guid.NewGuid(),
                VehicleId = vehicle.Id,
                ServiceProviderId = serviceProvider.Id,
                Description = "Oil change needed",
                RequestedServiceDate = DateTime.Now.AddDays(1),
                Priority = ServicePriority.Medium,
                Status = ServiceRequestStatus.Pending,
                ServiceType = ServiceType.Maintenance,
                Location = "123 Main St"
            };
            _context.ServiceRequests.Add(serviceRequest);

            _context.SaveChanges();
        }

        public ServiceRequestControllerTests()
        {
            _authToken = GetAuthTokenAsync(TestEmail, TestPassword).GetAwaiter().GetResult();
            SetAuthToken(_authToken);
        }

        [Fact]
        public async Task CreateServiceRequest_WithValidData_ReturnsCreated()
        {
            // Arrange
            var request = new
            {
                VehicleId = _vehicleId,
                Description = "New service request",
                RequestedServiceDate = DateTime.Now.AddDays(2),
                Priority = ServicePriority.High,
                ServiceType = ServiceType.Repair,
                Location = "456 Oak St"
            };

            // Act
            var response = await _client.PostAsync("/api/servicerequests",
                new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(content);
            Assert.NotNull(result.GetProperty("id").GetString());
        }

        [Fact]
        public async Task CreateServiceRequest_WithPastDate_ReturnsBadRequest()
        {
            // Arrange
            var request = new
            {
                VehicleId = _vehicleId,
                Description = "New service request",
                RequestedServiceDate = DateTime.Now.AddDays(-1), // Past date
                Priority = ServicePriority.High,
                ServiceType = ServiceType.Repair,
                Location = "456 Oak St"
            };

            // Act
            var response = await _client.PostAsync("/api/servicerequests",
                new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetServiceRequest_WithValidId_ReturnsRequest()
        {
            // Arrange
            var request = await _context.ServiceRequests.FirstAsync();

            // Act
            var response = await _client.GetAsync($"/api/servicerequests/{request.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(content);
            Assert.Equal(request.Id.ToString(), result.GetProperty("id").GetString());
            Assert.Equal(request.Description, result.GetProperty("description").GetString());
        }

        [Fact]
        public async Task GetServiceRequest_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync($"/api/servicerequests/{Guid.NewGuid()}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetServiceRequestsByVehicle_ReturnsRequests()
        {
            // Act
            var response = await _client.GetAsync($"/api/servicerequests/vehicle/{_vehicleId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(content);
            Assert.True(result.GetArrayLength() > 0);
        }

        [Fact]
        public async Task UpdateServiceRequestStatus_WithValidData_ReturnsOk()
        {
            // Arrange
            var request = await _context.ServiceRequests.FirstAsync();
            var updateData = new
            {
                Status = ServiceRequestStatus.InProgress
            };

            // Act
            var response = await _client.PutAsync($"/api/servicerequests/{request.Id}/status",
                new StringContent(JsonSerializer.Serialize(updateData), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(content);
            Assert.Equal(ServiceRequestStatus.InProgress.ToString(), result.GetProperty("status").GetString());
        }

        [Fact]
        public async Task AssignServiceProvider_WithValidData_ReturnsOk()
        {
            // Arrange
            var request = await _context.ServiceRequests.FirstAsync();
            var assignData = new
            {
                ServiceProviderId = _serviceProviderId
            };

            // Act
            var response = await _client.PutAsync($"/api/servicerequests/{request.Id}/assign",
                new StringContent(JsonSerializer.Serialize(assignData), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(content);
            Assert.Equal(_serviceProviderId.ToString(), result.GetProperty("serviceProviderId").GetString());
        }

        [Fact]
        public async Task AssignServiceProvider_WithUnavailableProvider_ReturnsBadRequest()
        {
            // Arrange
            var request = await _context.ServiceRequests.FirstAsync();
            var provider = await _context.ServiceProviders.FindAsync(_serviceProviderId);
            provider.IsAvailable = false;
            await _context.SaveChangesAsync();

            var assignData = new
            {
                ServiceProviderId = _serviceProviderId
            };

            // Act
            var response = await _client.PutAsync($"/api/servicerequests/{request.Id}/assign",
                new StringContent(JsonSerializer.Serialize(assignData), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
} 