using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using FixMate.Domain.Entities;


namespace FixMate.Application.Interfaces.Persistence
{
    public interface IVehicleRepository
    {
        Task<Vehicle> GetByIdAsync(Guid id);
        Task<IEnumerable<Vehicle>> GetByUserIdAsync(Guid userId);
        Task<Vehicle> GetByLicensePlateAsync(string LP);
        Task<IEnumerable<Vehicle>> GetAllAsync();
        Task AddAsync(Vehicle vehicle);
        void Delete(Vehicle vehicle);
        void Update(Vehicle vehicle);
        Task<IEnumerable<ServiceRequest>> GetServiceHistoryAsync(Guid vehicleId);
    }
} 