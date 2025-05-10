using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FixMate.Application.Interfaces.Persistence;
using FixMate.Domain.Entities;
using FixMate.Domain.Enums;

namespace FixMate.Infrastructure.Persistence.Repositories
{
    public class ServiceProviderRepository : IServiceProviderRepository
    {
        private readonly FixMateDbContext _context;

        public ServiceProviderRepository(FixMateDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ServiceProvider>> GetByOwnerIdAsync(Guid id)
        {
            return await _context.ServiceProviders
                .Where(sp => sp.Id == id)
                .ToListAsync();
        }

        public async Task<ServiceProvider> GetByIdAsync(Guid id)
        {
            return await _context.ServiceProviders
                .Include(sp => sp.AssignedRequests)
                .FirstOrDefaultAsync(sp => sp.Id == id);
        }

        public async Task<ServiceProvider> GetByEmailAsync(string email)
        {
            return await _context.ServiceProviders
                .FirstOrDefaultAsync(sp => sp.Email == email);
        }

        public async Task<IEnumerable<ServiceProvider>> GetBySpecializationAsync(Specialization specialization)
        {
            return await _context.ServiceProviders
                .Where(sp => sp.Specialization == specialization)
                .ToListAsync();
        }

        public async Task<IEnumerable<ServiceRequest>> GetAssignedRequests(Guid id)
        {

            return await _context.ServiceRequests.
                Where(sr => sr.AssignedProviderId == id).
                ToListAsync();

        }

        public async Task<IEnumerable<ServiceProvider>> GetAllAsync()
        {
            return await _context.ServiceProviders.ToListAsync();
        }

        public async Task AddAsync(ServiceProvider provider)
        {
            await _context.ServiceProviders.AddAsync(provider);
        }

        public void Delete(ServiceProvider provider)
        {
            _context.ServiceProviders.Remove(provider);
        }

        public void Update(ServiceProvider provider)
        {
            _context.ServiceProviders.Update(provider);
        }
    }
}
