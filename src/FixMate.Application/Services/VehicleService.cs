using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using FixMate.Application.Interfaces.Services;
using FixMate.Application.Interfaces.Persistence;
using FixMate.Domain.Entities;
using FixMate.Domain.ValueObjects;
using FixMate.Application.DTOs;
using Microsoft.Extensions.Logging;

namespace FixMate.Application.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IUserRepository _userRepository;
        private readonly Interfaces.Persistence.IUnitOfWork _unitOfWork;
        private readonly ILogger<VehicleService> _logger;

        public VehicleService(
            IVehicleRepository vehicleRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ILogger<VehicleService> logger)
        {
            _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<VehicleDto> CreateVehicleAsync(CreateVehicleDto vehicleDto)
        {
            if (vehicleDto == null)
                throw new ArgumentNullException(nameof(vehicleDto));

            try
            {
                // Check if vehicle with same license plate already exists
                var existingVehicle = await _vehicleRepository.GetByLicensePlateAsync(vehicleDto.LicensePlate);
                if (existingVehicle != null)
                    throw new ArgumentException("A vehicle with this license plate already exists", nameof(vehicleDto.LicensePlate));

                // Verify owner exists
                var owner = await _userRepository.GetByIdAsync(vehicleDto.OwnerId);
                if (owner == null)
                    throw new ArgumentException("Owner not found", nameof(vehicleDto.OwnerId));

                var vehicle = new Vehicle
                {
                    Make = vehicleDto.Make,
                    Model = vehicleDto.Model,
                    Year = vehicleDto.Year,
                    LicensePlate = new LicensePlate(vehicleDto.LicensePlate),
                    OwnerId = vehicleDto.OwnerId,
                    ServiceRequests = new List<ServiceRequest>()
                };

                await _vehicleRepository.AddAsync(vehicle);
                await _unitOfWork.SaveChangesAsync();
                
                return MapToDto(vehicle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating vehicle with license plate {LicensePlate}", vehicleDto.LicensePlate);
                throw;
            }
        }

        public async Task<VehicleDto> GetVehicleByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid vehicle ID", nameof(id));

            try
            {
                var vehicle = await _vehicleRepository.GetByIdAsync(id);
                if (vehicle == null)
                    return null;

                return MapToDto(vehicle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting vehicle with ID {VehicleId}", id);
                throw;
            }
        }

        public async Task<VehicleDto> GetVehicleByLicensePlateAsync(string licensePlate)
        {
            if (string.IsNullOrEmpty(licensePlate))
                throw new ArgumentException("License plate cannot be empty", nameof(licensePlate));

            try
            {
                var vehicle = await _vehicleRepository.GetByLicensePlateAsync(licensePlate);
                if (vehicle == null)
                    return null;

                return MapToDto(vehicle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting vehicle with license plate {LicensePlate}", licensePlate);
                throw;
            }
        }

        public async Task<IEnumerable<VehicleDto>> GetVehiclesByOwnerIdAsync(Guid ownerId)
        {
            if (ownerId == Guid.Empty)
                throw new ArgumentException("Invalid owner ID", nameof(ownerId));

            try
            {
                var vehicles = await _vehicleRepository.GetByOwnerIdAsync(ownerId);
                return vehicles.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting vehicles for owner {OwnerId}", ownerId);
                throw;
            }
        }

        public async Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync()
        {
            try
            {
                var vehicles = await _vehicleRepository.GetAllAsync();
                return vehicles.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all vehicles");
                throw;
            }
        }

        public async Task<VehicleDto> UpdateVehicleAsync(Guid id, UpdateVehicleDto vehicleDto)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid vehicle ID", nameof(id));
            if (vehicleDto == null)
                throw new ArgumentNullException(nameof(vehicleDto));

            try
            {
                var existingVehicle = await _vehicleRepository.GetByIdAsync(id);
                if (existingVehicle == null)
                    throw new ArgumentException("Vehicle not found", nameof(id));

                existingVehicle.Make = vehicleDto.Make;
                existingVehicle.Model = vehicleDto.Model;
                existingVehicle.Year = vehicleDto.Year;

                _vehicleRepository.Update(existingVehicle);
                await _unitOfWork.SaveChangesAsync();
                
                return MapToDto(existingVehicle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating vehicle with ID {VehicleId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteVehicleAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid vehicle ID", nameof(id));

            try
            {
                var vehicle = await _vehicleRepository.GetByIdAsync(id);
                if (vehicle == null)
                    return false;

                _vehicleRepository.Delete(vehicle);
                await _unitOfWork.SaveChangesAsync();
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting vehicle with ID {VehicleId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<ServiceRequestDto>> GetVehicleServiceRequestsAsync(Guid vehicleId)
        {
            if (vehicleId == Guid.Empty)
                throw new ArgumentException("Invalid vehicle ID", nameof(vehicleId));

            try
            {
                var requests = await _vehicleRepository.GetServiceRequestsAsync(vehicleId);
                return requests.Select(sr => new ServiceRequestDto
                {
                    Id = sr.Id,
                    VehicleId = sr.VehicleId,
                    ServiceType = sr.ServiceType,
                    Status = sr.Status,
                    Notes = sr.Notes,
                    RequestedAt = sr.RequestedAt,
                    CompletedAt = sr.CompletedAt,
                    AssignedProviderId = sr.AssignedProviderId,
                    AssignedProviderName = sr.AssignedProvider?.FullName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting service requests for vehicle {VehicleId}", vehicleId);
                throw;
            }
        }

        private static VehicleDto MapToDto(Vehicle vehicle)
        {
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle));

            return new VehicleDto
            {
                Id = vehicle.Id,
                Make = vehicle.Make ?? throw new InvalidOperationException("Vehicle make cannot be null"),
                Model = vehicle.Model ?? throw new InvalidOperationException("Vehicle model cannot be null"),
                Year = vehicle.Year,
                LicensePlate = vehicle.LicensePlate?.ToString() ?? throw new InvalidOperationException("Vehicle license plate cannot be null"),
                OwnerId = vehicle.OwnerId,
                OwnerName = vehicle.Owner?.FullName
            };
        }
    }
} 