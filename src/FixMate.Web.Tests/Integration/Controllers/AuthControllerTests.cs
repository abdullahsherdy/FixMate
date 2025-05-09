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
    public class AuthControllerTests : TestBase
    {
        private const string TestEmail = "test@example.com";
        private const string TestPassword = "Test123!";

        protected override void SeedTestData()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = "Test User",
                Email = new Email(TestEmail),
                PhoneNumber = "+1234567890",
                PasswordHash = TestPassword // In a real app, this would be hashed
            };

            _context.Users.Add(user);
            _context.SaveChanges();
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsToken()
        {
            // Arrange
            var loginRequest = new
            {
                Email = TestEmail,
                Password = TestPassword
            };

            // Act
            var response = await _client.PostAsync("/api/auth/login",
                new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(content);
            Assert.NotNull(result.GetProperty("token").GetString());
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new
            {
                Email = TestEmail,
                Password = "WrongPassword"
            };

            // Act
            var response = await _client.PostAsync("/api/auth/login",
                new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Register_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var registerRequest = new
            {
                FullName = "New User",
                Email = "newuser@example.com",
                PhoneNumber = "+1987654321",
                Password = "NewUser123!"
            };

            // Act
            var response = await _client.PostAsync("/api/auth/register",
                new StringContent(JsonSerializer.Serialize(registerRequest), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(content);
            Assert.Equal("User registered successfully", result.GetProperty("message").GetString());
        }

        [Fact]
        public async Task Register_WithExistingEmail_ReturnsBadRequest()
        {
            // Arrange
            var registerRequest = new
            {
                FullName = "Test User",
                Email = TestEmail, // Using existing email
                PhoneNumber = "+1234567890",
                Password = "Test123!"
            };

            // Act
            var response = await _client.PostAsync("/api/auth/register",
                new StringContent(JsonSerializer.Serialize(registerRequest), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(content);
            Assert.Equal("User with this email already exists", result.GetProperty("message").GetString());
        }

        [Fact]
        public async Task Register_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var registerRequest = new
            {
                FullName = "", // Invalid: empty name
                Email = "invalid-email", // Invalid email format
                PhoneNumber = "invalid-phone", // Invalid phone format
                Password = "123" // Invalid: too short
            };

            // Act
            var response = await _client.PostAsync("/api/auth/register",
                new StringContent(JsonSerializer.Serialize(registerRequest), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
} 