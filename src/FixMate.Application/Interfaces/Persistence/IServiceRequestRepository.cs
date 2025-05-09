using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FixMate.Domain.Entities;

namespace FixMate.Application.Interfaces.Persistence
{
    public interface IServiceRequestRepository
    {
        Task<ServiceRequest> GetByIdAsync(Guid id);
        Task<IEnumerable<ServiceRequest>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<ServiceRequest>> GetByMechanicIdAsync(Guid mechanicId);
        Task AddAsync(ServiceRequest serviceRequest);
        void Delete(ServiceRequest serviceRequest);
        void Update(ServiceRequest serviceRequest);
    }
} 