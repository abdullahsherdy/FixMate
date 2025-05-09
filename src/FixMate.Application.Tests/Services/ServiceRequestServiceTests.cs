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
    public class ServiceRequestServiceTests
    {
        private readonly Mock<IServiceRequestRepository> _serviceRequestRepositoryMock;
        private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
        private readonly Mock<IServiceProviderRepository> _serviceProviderRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ServiceRequestService _serviceRequestService;

        public ServiceRequestServiceTests()
        {
            _serviceRequestRepositoryMock = new Mock<IServiceRequestRepository>();
            _vehicleRepositoryMock = new Mock<IVehicleRepository>();
            _serviceProviderRepositoryMock = new Mock<IServiceProviderRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _serviceRequestService = new ServiceRequestService(
                _serviceRequestRepositoryMock.Object,
                _vehicleRepositoryMock.Object,
                _serviceProviderRepositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task CreateServiceRequestAsync_WithValidRequest_ShouldCreateRequest()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();
            var request = new ServiceRequest
            {
                Id = Guid.NewGuid(),
                VehicleId = vehicleId,
                Description = "Oil change needed",
                RequestedServiceDate = DateTime.Now.AddDays(1),
                Priority = ServicePriority.Medium,
                Status = ServiceRequestStatus.Pending,
                ServiceType = ServiceType.Maintenance,
                Location = "123 Main St"
            };

            _vehicleRepositoryMock.Setup(x => x.GetByIdAsync(vehicleId))
                .ReturnsAsync(new Vehicle { Id = vehicleId });

            // Act
            var result = await _serviceRequestService.CreateServiceRequestAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Id, result.Id);
            Assert.Equal(request.Description, result.Description);
            _serviceRequestRepositoryMock.Verify(x => x.AddAsync(request), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateServiceRequestAsync_WithInvalidVehicle_ShouldThrowException()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();
            var request = new ServiceRequest
            {
                Id = Guid.NewGuid(),
                VehicleId = vehicleId,
                Description = "Oil change needed",
                RequestedServiceDate = DateTime.Now.AddDays(1),
                Priority = ServicePriority.Medium,
                Status = ServiceRequestStatus.Pending,
                ServiceType = ServiceType.Maintenance,
                Location = "123 Main St"
            };

            _vehicleRepositoryMock.Setup(x => x.GetByIdAsync(vehicleId))
                .ReturnsAsync((Vehicle)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _serviceRequestService.CreateServiceRequestAsync(request));
        }

        [Fact]
        public async Task CreateServiceRequestAsync_WithPastServiceDate_ShouldThrowException()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();
            var request = new ServiceRequest
            {
                Id = Guid.NewGuid(),
                VehicleId = vehicleId,
                Description = "Oil change needed",
                RequestedServiceDate = DateTime.Now.AddDays(-1), // Past date
                Priority = ServicePriority.Medium,
                Status = ServiceRequestStatus.Pending,
                ServiceType = ServiceType.Maintenance,
                Location = "123 Main St"
            };

            _vehicleRepositoryMock.Setup(x => x.GetByIdAsync(vehicleId))
                .ReturnsAsync(new Vehicle { Id = vehicleId });

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _serviceRequestService.CreateServiceRequestAsync(request));
        }

        [Fact]
        public async Task AssignServiceProviderAsync_WithValidData_ShouldAssignProvider()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var providerId = Guid.NewGuid();
            var request = new ServiceRequest
            {
                Id = requestId,
                Status = ServiceRequestStatus.Pending
            };
            var provider = new ServiceProvider
            {
                Id = providerId,
                IsAvailable = true
            };

            _serviceRequestRepositoryMock.Setup(x => x.GetByIdAsync(requestId))
                .ReturnsAsync(request);
            _serviceProviderRepositoryMock.Setup(x => x.GetByIdAsync(providerId))
                .ReturnsAsync(provider);

            // Act
            var result = await _serviceRequestService.AssignServiceProviderAsync(requestId, providerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(providerId, result.ServiceProviderId);
            Assert.Equal(ServiceRequestStatus.Assigned, result.Status);
            _serviceRequestRepositoryMock.Verify(x => x.Update(request), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AssignServiceProviderAsync_WithUnavailableProvider_ShouldThrowException()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var providerId = Guid.NewGuid();
            var request = new ServiceRequest
            {
                Id = requestId,
                Status = ServiceRequestStatus.Pending
            };
            var provider = new ServiceProvider
            {
                Id = providerId,
                IsAvailable = false
            };

            _serviceRequestRepositoryMock.Setup(x => x.GetByIdAsync(requestId))
                .ReturnsAsync(request);
            _serviceProviderRepositoryMock.Setup(x => x.GetByIdAsync(providerId))
                .ReturnsAsync(provider);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _serviceRequestService.AssignServiceProviderAsync(requestId, providerId));
        }

        [Fact]
        public async Task UpdateServiceRequestStatusAsync_WithValidData_ShouldUpdateStatus()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var request = new ServiceRequest
            {
                Id = requestId,
                Status = ServiceRequestStatus.Assigned
            };

            _serviceRequestRepositoryMock.Setup(x => x.GetByIdAsync(requestId))
                .ReturnsAsync(request);

            // Act
            var result = await _serviceRequestService.UpdateServiceRequestStatusAsync(
                requestId, ServiceRequestStatus.InProgress);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ServiceRequestStatus.InProgress, result.Status);
            _serviceRequestRepositoryMock.Verify(x => x.Update(request), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateServiceRequestStatusAsync_WithInvalidStatusTransition_ShouldThrowException()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var request = new ServiceRequest
            {
                Id = requestId,
                Status = ServiceRequestStatus.Pending
            };

            _serviceRequestRepositoryMock.Setup(x => x.GetByIdAsync(requestId))
                .ReturnsAsync(request);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _serviceRequestService.UpdateServiceRequestStatusAsync(
                    requestId, ServiceRequestStatus.Completed));
        }

        [Fact]
        public async Task GetServiceRequestsByVehicleIdAsync_ShouldReturnRequests()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();
            var requests = new List<ServiceRequest>
            {
                new ServiceRequest
                {
                    Id = Guid.NewGuid(),
                    VehicleId = vehicleId,
                    Status = ServiceRequestStatus.Completed
                },
                new ServiceRequest
                {
                    Id = Guid.NewGuid(),
                    VehicleId = vehicleId,
                    Status = ServiceRequestStatus.Pending
                }
            };

            _serviceRequestRepositoryMock.Setup(x => x.GetByVehicleIdAsync(vehicleId))
                .ReturnsAsync(requests);

            // Act
            var result = await _serviceRequestService.GetServiceRequestsByVehicleIdAsync(vehicleId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, r => Assert.Equal(vehicleId, r.VehicleId));
        }

        [Fact]
        public async Task GetServiceRequestsByVehicleIdAsync_WithNoRequests_ShouldReturnEmptyList()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();
            _serviceRequestRepositoryMock.Setup(x => x.GetByVehicleIdAsync(vehicleId))
                .ReturnsAsync(new List<ServiceRequest>());

            // Act
            var result = await _serviceRequestService.GetServiceRequestsByVehicleIdAsync(vehicleId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task UpdateServiceRequestStatusAsync_WithNonExistentRequest_ShouldThrowException()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            _serviceRequestRepositoryMock.Setup(x => x.GetByIdAsync(requestId))
                .ReturnsAsync((ServiceRequest)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _serviceRequestService.UpdateServiceRequestStatusAsync(
                    requestId, ServiceRequestStatus.InProgress));
        }
    }
} 