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
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<VehicleService> _logger;

        public VehicleService(
            IVehicleRepository vehicleRepository,
            IUnitOfWork unitOfWork,
            ILogger<VehicleService> logger)
        {
            _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<VehicleDto> CreateVehicleAsync(CreateVehicleDto vehicleDto)
        {
            if (vehicleDto == null)
                throw new ArgumentNullException(nameof(vehicleDto));

            try
            {
                // Validate vehicle data
                if (string.IsNullOrEmpty(vehicleDto.Make) || string.IsNullOrEmpty(vehicleDto.Model))
                    throw new ArgumentException("Make and Model are required");

                var vehicle = new Vehicle
                {
                    Make = vehicleDto.Make,
                    Model = vehicleDto.Model,
                    Type = vehicleDto.Type,
                    LicensePlate = new LicensePlate(vehicleDto.LicensePlate),
                    UserId = vehicleDto.UserId
                };

                await _vehicleRepository.AddAsync(vehicle);
                await _unitOfWork.SaveChangesAsync();
                
                return MapToDto(vehicle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating vehicle for user {UserId}", vehicleDto.UserId);
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

        public async Task<IEnumerable<VehicleDto>> GetVehiclesByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Invalid user ID", nameof(userId));

            try
            {
                var vehicles = await _vehicleRepository.GetByUserIdAsync(userId);
                return vehicles.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting vehicles for user {UserId}", userId);
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
                existingVehicle.Type = vehicleDto.Type;
                existingVehicle.LicensePlate = new LicensePlate(vehicleDto.LicensePlate);

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

        public async Task<IEnumerable<ServiceRequestDto>> GetVehicleServiceHistoryAsync(Guid vehicleId)
        {
            if (vehicleId == Guid.Empty)
                throw new ArgumentException("Invalid vehicle ID", nameof(vehicleId));

            try
            {
                var serviceRequests = await _vehicleRepository.GetServiceHistoryAsync(vehicleId);
                return serviceRequests.Select(sr => new ServiceRequestDto
                {
                    Id = sr.Id,
                    VehicleId = sr.VehicleId,
                    ServiceType = sr.ServiceType,
                    Status = sr.Status,
                    Notes = sr.Notes,
                    RequestedAt = sr.RequestedAt,
                    CompletedAt = sr.CompletedAt,
                    AssignedProviderId = sr.AssignedProviderId,
                    AssignedProviderName = sr.AssignedProvider != null ? 
                        sr.AssignedProvider.FullName : null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting service history for vehicle {VehicleId}", vehicleId);
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
                Type = vehicle.Type,
                LicensePlate = vehicle.LicensePlate?.Value ?? throw new InvalidOperationException("Vehicle license plate cannot be null"),
                UserId = vehicle.UserId,
                OwnerName = vehicle.Owner != null ? 
                    $"{vehicle.Owner.FirstName} {vehicle.Owner.LastName}" : null
            };
        }
    }
} 