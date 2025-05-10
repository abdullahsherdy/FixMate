using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FixMate.Domain.Entities;
using FixMate.Domain.Enums;

namespace FixMate.Application.Interfaces.Persistence
{
    public interface IServiceProviderRepository
    {
        Task<IEnumerable<ServiceProvider>> GetByOwnerIdAsync(Guid id);
        Task<ServiceProvider> GetByIdAsync(Guid id);

        Task<ServiceProvider> GetByEmailAsync(string email);
        Task<IEnumerable<ServiceProvider>> GetBySpecializationAsync(Specialization specialization);
        Task<IEnumerable<ServiceProvider>> GetAllAsync();
        Task AddAsync(ServiceProvider provider);
        void Delete(ServiceProvider provider);
        void Update(ServiceProvider provider);
    }
} 