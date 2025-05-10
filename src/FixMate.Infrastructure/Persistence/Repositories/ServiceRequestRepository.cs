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

        public async Task<IEnumerable<ServiceRequest>> GetByOwnerIdAsync(Guid id)
        {
            return await _context.ServiceRequests
                .Include(sr => sr.Vehicle)
                .Include(sr => sr.AssignedProvider)
                .Where(sr => sr.Vehicle.OwnerId == id)
                .ToListAsync();
        }
       

        public async Task AddAsync(ServiceRequest serviceRequest)
        {
            if (serviceRequest == null)
                throw new ArgumentNullException(nameof(serviceRequest));

           

            await _context.ServiceRequests.AddAsync(serviceRequest);
        }

        public void Delete(ServiceRequest serviceRequest)
        {
            if (_context.ServiceRequests.Local.Any(e => e.Id == serviceRequest.Id))
            {
                _context.Entry(serviceRequest).State = EntityState.Detached;
            }
        }

        public void Update(ServiceRequest serviceRequest)
        {
            _context.Entry(serviceRequest).State = EntityState.Modified;
        }

        public async Task<IEnumerable<ServiceRequest>> GetAllAsync()
        {
            return await _context.ServiceRequests
                .Include(sr => sr.Vehicle)
                .Include(sr => sr.AssignedProvider)
                .ToListAsync(); 
        }

        public async Task<IEnumerable<ServiceRequest>> GetByVehicleIdAsync(Guid vehicleId)
        {
            return await _context.ServiceRequests
               .Include(sr => sr.Vehicle)
               .Include(sr => sr.AssignedProvider)
               .Where(sr => sr.VehicleId== vehicleId)
               .ToListAsync();
        }

        public async Task<IEnumerable<ServiceRequest>> GetByProviderIdAsync(Guid providerId)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceRequest> GetByIdAsync(Guid id)
        {
            return await _context.ServiceRequests.FirstOreDefaultAsync(sr => sr.Id == id);
        }
    }
} 