using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FixMate.Application.DTOs;

namespace FixMate.Application.Interfaces.Services
{
    public interface IVehicleService
    {
        Task<VehicleDto> CreateVehicleAsync(CreateVehicleDto vehicleDto);
        Task<VehicleDto> GetVehicleByIdAsync(Guid id);
        Task<IEnumerable<VehicleDto>> GetVehiclesByUserIdAsync(Guid userId);
        Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync();
        Task<VehicleDto> UpdateVehicleAsync(Guid id, UpdateVehicleDto vehicleDto);
        Task<bool> DeleteVehicleAsync(Guid id);
        Task<IEnumerable<ServiceRequestDto>> GetVehicleServiceHistoryAsync(Guid vehicleId);
    }
} 