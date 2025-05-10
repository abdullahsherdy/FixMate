using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FixMate.Domain.Entities;

namespace FixMate.Application.Interfaces.Persistence
{
    public interface IServiceRequestRepository
    {
        Task<ServiceRequest> GetByIdAsync(Guid id);
        Task<IEnumerable<ServiceRequest>> GetAllAsync();
        Task<IEnumerable<ServiceRequest>> GetByVehicleIdAsync(Guid vehicleId); // Vehicle 
        Task<IEnumerable<ServiceRequest>> GetByOwnerIdAsync(Guid vehicleId); // user 
        Task<IEnumerable<ServiceRequest>> GetByProviderIdAsync(Guid providerId);  /// Mechanic 
        Task AddAsync(ServiceRequest request);
        void Update(ServiceRequest request);
        void Delete(ServiceRequest request);
    }
} 