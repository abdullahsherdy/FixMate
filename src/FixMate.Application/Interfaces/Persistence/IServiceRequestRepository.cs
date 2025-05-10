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
        Task<IEnumerable<ServiceRequest>> GetByVehicleIdAsync(Guid vehicleId);
        Task<IEnumerable<ServiceRequest>> GetByOwnerIdAsync(Guid vehicleId);

        Task<IEnumerable<ServiceRequest>> GetByProviderIdAsync(Guid providerId);
        Task AddAsync(ServiceRequest request);
        void Update(ServiceRequest request);
        void Delete(ServiceRequest request);
    }
} 