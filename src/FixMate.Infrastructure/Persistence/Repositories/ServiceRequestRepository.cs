using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FixMate.Application.Interfaces.Persistence;
using FixMate.Domain.Entities;

namespace FixMate.Infrastructure.Persistence.Repositories
{
    public class ServiceRequestRepository : IServiceRequestRepository
    {
        private readonly FixMateDbContext _context;

        public ServiceRequestRepository(FixMateDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceRequest> GetByIdAsync(Guid id)
        {
            return await _context.ServiceRequests
                .Include(sr => sr.Vehicle)
                .Include(sr => sr.AssignedProvider)
                .FirstOrDefaultAsync(sr => sr.Id == id);
        }

        public async Task<IEnumerable<ServiceRequest>> GetByUserIdAsync(Guid userId)
        {
            return await _context.ServiceRequests
                .Include(sr => sr.Vehicle)
                .Include(sr => sr.AssignedProvider)
                .Where(sr => sr.Vehicle.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ServiceRequest>> GetByMechanicIdAsync(Guid mechanicId)
        {
            return await _context.ServiceRequests
                .Include(sr => sr.Vehicle)
                .Include(sr => sr.AssignedProvider)
                .Where(sr => sr.AssignedProviderId == mechanicId)
                .ToListAsync();
        }

        public async Task AddAsync(ServiceRequest serviceRequest)
        {
            await _context.ServiceRequests.AddAsync(serviceRequest);
        }

        public void Delete(ServiceRequest serviceRequest)
        {
            _context.ServiceRequests.Remove(serviceRequest);
        }

        public void Update(ServiceRequest serviceRequest)
        {
            _context.Entry(serviceRequest).State = EntityState.Modified;
        }
    }
} 