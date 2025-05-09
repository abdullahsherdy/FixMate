using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FixMate.Application.Services;
using FixMate.Application.Interfaces.Persistence;
using FixMate.Domain.Entities;
using FixMate.Domain.Enums;

namespace FixMate.Application.Tests.Services
{
    public class ServiceProviderServiceTests
    {
        private readonly Mock<IServiceProviderRepository> _serviceProviderRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ServiceProviderService _serviceProviderService;

        public ServiceProviderServiceTests()
        {
            _serviceProviderRepositoryMock = new Mock<IServiceProviderRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _serviceProviderService = new ServiceProviderService(
                _serviceProviderRepositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task CreateServiceProviderAsync_WithValidProvider_ShouldCreateProvider()
        {
            // Arrange
            var provider = new ServiceProvider
            {
                Id = Guid.NewGuid(),
                FullName = "John Smith",
                Email = "john@example.com",
                PhoneNumber = "+1234567890",
                Specialization = Specialization.General,
                BusinessName = "John's Auto Service",
                BusinessAddress = "123 Main St",
                BusinessPhone = "+1234567890",
                BusinessEmail = "business@example.com",
                BusinessLicense = "LIC123",
                InsurancePolicy = "INS456"
            };

            _serviceProviderRepositoryMock.Setup(x => x.GetByEmailAsync(provider.Email))
                .ReturnsAsync((ServiceProvider)null);

            // Act
            var result = await _serviceProviderService.CreateServiceProviderAsync(provider);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(provider.Id, result.Id);
            Assert.Equal(provider.BusinessName, result.BusinessName);
            _serviceProviderRepositoryMock.Verify(x => x.AddAsync(provider), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateServiceProviderAsync_WithExistingEmail_ShouldThrowException()
        {
            // Arrange
            var provider = new ServiceProvider
            {
                Id = Guid.NewGuid(),
                FullName = "John Smith",
                Email = "john@example.com",
                PhoneNumber = "+1234567890",
                Specialization = Specialization.General,
                BusinessName = "John's Auto Service",
                BusinessAddress = "123 Main St",
                BusinessPhone = "+1234567890",
                BusinessEmail = "business@example.com",
                BusinessLicense = "LIC123",
                InsurancePolicy = "INS456"
            };

            _serviceProviderRepositoryMock.Setup(x => x.GetByEmailAsync(provider.Email))
                .ReturnsAsync(provider);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _serviceProviderService.CreateServiceProviderAsync(provider));
        }

        [Fact]
        public async Task GetServiceProviderByIdAsync_WithValidId_ShouldReturnProvider()
        {
            // Arrange
            var providerId = Guid.NewGuid();
            var provider = new ServiceProvider
            {
                Id = providerId,
                FullName = "John Smith",
                Email = "john@example.com",
                PhoneNumber = "+1234567890",
                Specialization = Specialization.General,
                BusinessName = "John's Auto Service"
            };

            _serviceProviderRepositoryMock.Setup(x => x.GetByIdAsync(providerId))
                .ReturnsAsync(provider);

            // Act
            var result = await _serviceProviderService.GetServiceProviderByIdAsync(providerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(providerId, result.Id);
            Assert.Equal(provider.FullName, result.FullName);
        }

        [Fact]
        public async Task GetServiceProvidersBySpecializationAsync_ShouldReturnProviders()
        {
            // Arrange
            var specialization = Specialization.General;
            var providers = new List<ServiceProvider>
            {
                new ServiceProvider
                {
                    Id = Guid.NewGuid(),
                    FullName = "John Smith",
                    Email = "john@example.com",
                    Specialization = specialization
                },
                new ServiceProvider
                {
                    Id = Guid.NewGuid(),
                    FullName = "Jane Doe",
                    Email = "jane@example.com",
                    Specialization = specialization
                }
            };

            _serviceProviderRepositoryMock.Setup(x => x.GetBySpecializationAsync(specialization))
                .ReturnsAsync(providers);

            // Act
            var result = await _serviceProviderService.GetServiceProvidersBySpecializationAsync(specialization);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, p => Assert.Equal(specialization, p.Specialization));
        }

        [Fact]
        public async Task UpdateServiceProviderAsync_WithValidProvider_ShouldUpdateProvider()
        {
            // Arrange
            var provider = new ServiceProvider
            {
                Id = Guid.NewGuid(),
                FullName = "John Smith",
                Email = "john@example.com",
                PhoneNumber = "+1234567890",
                Specialization = Specialization.General,
                BusinessName = "John's Auto Service"
            };

            _serviceProviderRepositoryMock.Setup(x => x.GetByIdAsync(provider.Id))
                .ReturnsAsync(provider);

            // Act
            var result = await _serviceProviderService.UpdateServiceProviderAsync(provider);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(provider.Id, result.Id);
            _serviceProviderRepositoryMock.Verify(x => x.Update(provider), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAvailabilityAsync_WithValidData_ShouldUpdateAvailability()
        {
            // Arrange
            var providerId = Guid.NewGuid();
            var provider = new ServiceProvider
            {
                Id = providerId,
                IsAvailable = true
            };

            _serviceProviderRepositoryMock.Setup(x => x.GetByIdAsync(providerId))
                .ReturnsAsync(provider);

            // Act
            var result = await _serviceProviderService.UpdateAvailabilityAsync(providerId, false);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsAvailable);
            _serviceProviderRepositoryMock.Verify(x => x.Update(provider), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateServiceProviderAsync_WithInvalidEmail_ShouldThrowException()
        {
            // Arrange
            var provider = new ServiceProvider
            {
                Id = Guid.NewGuid(),
                FullName = "John Smith",
                Email = "invalid-email", // Invalid email format
                PhoneNumber = "+1234567890",
                Specialization = Specialization.General,
                BusinessName = "John's Auto Service",
                BusinessAddress = "123 Main St",
                BusinessPhone = "+1234567890",
                BusinessEmail = "business@example.com",
                BusinessLicense = "LIC123",
                InsurancePolicy = "INS456"
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _serviceProviderService.CreateServiceProviderAsync(provider));
        }

        [Fact]
        public async Task CreateServiceProviderAsync_WithInvalidPhoneNumber_ShouldThrowException()
        {
            // Arrange
            var provider = new ServiceProvider
            {
                Id = Guid.NewGuid(),
                FullName = "John Smith",
                Email = "john@example.com",
                PhoneNumber = "invalid-phone", // Invalid phone format
                Specialization = Specialization.General,
                BusinessName = "John's Auto Service",
                BusinessAddress = "123 Main St",
                BusinessPhone = "+1234567890",
                BusinessEmail = "business@example.com",
                BusinessLicense = "LIC123",
                InsurancePolicy = "INS456"
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _serviceProviderService.CreateServiceProviderAsync(provider));
        }

        [Fact]
        public async Task UpdateServiceProviderAsync_WithNonExistentProvider_ShouldThrowException()
        {
            // Arrange
            var provider = new ServiceProvider
            {
                Id = Guid.NewGuid(),
                FullName = "John Smith",
                Email = "john@example.com",
                PhoneNumber = "+1234567890",
                Specialization = Specialization.General,
                BusinessName = "John's Auto Service"
            };

            _serviceProviderRepositoryMock.Setup(x => x.GetByIdAsync(provider.Id))
                .ReturnsAsync((ServiceProvider)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _serviceProviderService.UpdateServiceProviderAsync(provider));
        }

        [Fact]
        public async Task GetServiceProvidersBySpecializationAsync_WithNoProviders_ShouldReturnEmptyList()
        {
            // Arrange
            var specialization = Specialization.General;
            _serviceProviderRepositoryMock.Setup(x => x.GetBySpecializationAsync(specialization))
                .ReturnsAsync(new List<ServiceProvider>());

            // Act
            var result = await _serviceProviderService.GetServiceProvidersBySpecializationAsync(specialization);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task UpdateAvailabilityAsync_WithNonExistentProvider_ShouldThrowException()
        {
            // Arrange
            var providerId = Guid.NewGuid();
            _serviceProviderRepositoryMock.Setup(x => x.GetByIdAsync(providerId))
                .ReturnsAsync((ServiceProvider)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _serviceProviderService.UpdateAvailabilityAsync(providerId, false));
        }
    }
} 