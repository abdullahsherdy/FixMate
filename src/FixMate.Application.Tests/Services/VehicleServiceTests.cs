using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FixMate.Application.Services;
using FixMate.Application.Interfaces.Persistence;
using FixMate.Domain.Entities;

namespace FixMate.Application.Tests.Services
{
    public class VehicleServiceTests
    {
        private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly VehicleService _vehicleService;

        public VehicleServiceTests()
        {
            _vehicleRepositoryMock = new Mock<IVehicleRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _vehicleService = new VehicleService(_vehicleRepositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task CreateVehicleAsync_WithValidVehicle_ShouldCreateVehicle()
        {
            // Arrange
            var vehicle = new Vehicle
            {
                Id = Guid.NewGuid(),
                Make = "Toyota",
                Model = "Camry",
                Year = 2020,
                LicensePlate = "ABC123",
                VIN = "1HGCM82633A123456",
                OwnerId = Guid.NewGuid()
            };

            _vehicleRepositoryMock.Setup(x => x.GetByVINAsync(vehicle.VIN))
                .ReturnsAsync((Vehicle)null);

            // Act
            var result = await _vehicleService.CreateVehicleAsync(vehicle);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(vehicle.Id, result.Id);
            Assert.Equal(vehicle.Make, result.Make);
            _vehicleRepositoryMock.Verify(x => x.AddAsync(vehicle), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateVehicleAsync_WithExistingVIN_ShouldThrowException()
        {
            // Arrange
            var vehicle = new Vehicle
            {
                Id = Guid.NewGuid(),
                Make = "Toyota",
                Model = "Camry",
                Year = 2020,
                LicensePlate = "ABC123",
                VIN = "1HGCM82633A123456",
                OwnerId = Guid.NewGuid()
            };

            _vehicleRepositoryMock.Setup(x => x.GetByVINAsync(vehicle.VIN))
                .ReturnsAsync(vehicle);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _vehicleService.CreateVehicleAsync(vehicle));
        }

        [Fact]
        public async Task GetVehicleByIdAsync_WithValidId_ShouldReturnVehicle()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();
            var vehicle = new Vehicle
            {
                Id = vehicleId,
                Make = "Toyota",
                Model = "Camry",
                Year = 2020,
                LicensePlate = "ABC123",
                VIN = "1HGCM82633A123456",
                OwnerId = Guid.NewGuid()
            };

            _vehicleRepositoryMock.Setup(x => x.GetByIdAsync(vehicleId))
                .ReturnsAsync(vehicle);

            // Act
            var result = await _vehicleService.GetVehicleByIdAsync(vehicleId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(vehicleId, result.Id);
            Assert.Equal(vehicle.Make, result.Make);
        }

        [Fact]
        public async Task GetVehiclesByOwnerIdAsync_ShouldReturnVehicles()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var vehicles = new List<Vehicle>
            {
                new Vehicle
                {
                    Id = Guid.NewGuid(),
                    Make = "Toyota",
                    Model = "Camry",
                    Year = 2020,
                    LicensePlate = "ABC123",
                    VIN = "1HGCM82633A123456",
                    OwnerId = ownerId
                },
                new Vehicle
                {
                    Id = Guid.NewGuid(),
                    Make = "Honda",
                    Model = "Accord",
                    Year = 2021,
                    LicensePlate = "XYZ789",
                    VIN = "2HGCM82633A123456",
                    OwnerId = ownerId
                }
            };

            _vehicleRepositoryMock.Setup(x => x.GetByOwnerIdAsync(ownerId))
                .ReturnsAsync(vehicles);

            // Act
            var result = await _vehicleService.GetVehiclesByOwnerIdAsync(ownerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, v => Assert.Equal(ownerId, v.OwnerId));
        }

        [Fact]
        public async Task UpdateVehicleAsync_WithValidVehicle_ShouldUpdateVehicle()
        {
            // Arrange
            var vehicle = new Vehicle
            {
                Id = Guid.NewGuid(),
                Make = "Toyota",
                Model = "Camry",
                Year = 2020,
                LicensePlate = "ABC123",
                VIN = "1HGCM82633A123456",
                OwnerId = Guid.NewGuid()
            };

            _vehicleRepositoryMock.Setup(x => x.GetByIdAsync(vehicle.Id))
                .ReturnsAsync(vehicle);

            // Act
            var result = await _vehicleService.UpdateVehicleAsync(vehicle);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(vehicle.Id, result.Id);
            _vehicleRepositoryMock.Verify(x => x.Update(vehicle), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateVehicleAsync_WithInvalidYear_ShouldThrowException()
        {
            // Arrange
            var vehicle = new Vehicle
            {
                Id = Guid.NewGuid(),
                Make = "Toyota",
                Model = "Camry",
                Year = 1800, // Invalid year
                LicensePlate = "ABC123",
                VIN = "1HGCM82633A123456",
                OwnerId = Guid.NewGuid()
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _vehicleService.CreateVehicleAsync(vehicle));
        }

        [Fact]
        public async Task CreateVehicleAsync_WithInvalidVIN_ShouldThrowException()
        {
            // Arrange
            var vehicle = new Vehicle
            {
                Id = Guid.NewGuid(),
                Make = "Toyota",
                Model = "Camry",
                Year = 2020,
                LicensePlate = "ABC123",
                VIN = "INVALID-VIN", // Invalid VIN format
                OwnerId = Guid.NewGuid()
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _vehicleService.CreateVehicleAsync(vehicle));
        }

        [Fact]
        public async Task UpdateVehicleAsync_WithNonExistentVehicle_ShouldThrowException()
        {
            // Arrange
            var vehicle = new Vehicle
            {
                Id = Guid.NewGuid(),
                Make = "Toyota",
                Model = "Camry",
                Year = 2020,
                LicensePlate = "ABC123",
                VIN = "1HGCM82633A123456",
                OwnerId = Guid.NewGuid()
            };

            _vehicleRepositoryMock.Setup(x => x.GetByIdAsync(vehicle.Id))
                .ReturnsAsync((Vehicle)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _vehicleService.UpdateVehicleAsync(vehicle));
        }

        [Fact]
        public async Task GetVehicleByIdAsync_WithNonExistentId_ShouldReturnNull()
        {
            // Arrange
            var vehicleId = Guid.NewGuid();
            _vehicleRepositoryMock.Setup(x => x.GetByIdAsync(vehicleId))
                .ReturnsAsync((Vehicle)null);

            // Act
            var result = await _vehicleService.GetVehicleByIdAsync(vehicleId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetVehiclesByOwnerIdAsync_WithNoVehicles_ShouldReturnEmptyList()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            _vehicleRepositoryMock.Setup(x => x.GetByOwnerIdAsync(ownerId))
                .ReturnsAsync(new List<Vehicle>());

            // Act
            var result = await _vehicleService.GetVehiclesByOwnerIdAsync(ownerId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
} 