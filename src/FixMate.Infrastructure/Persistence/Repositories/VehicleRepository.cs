using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FixMate.Application.Interfaces.Persistence;
using FixMate.Domain.Entities;

namespace FixMate.Infrastructure.Persistence.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly FixMateDbContext _context;

        public VehicleRepository(FixMateDbContext context)
        {
            _context = context;
        }

        public async Task<Vehicle> GetByIdAsync(Guid id)
        {
            return await _context.Vehicles
                .Include(v => v.Owner)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<IEnumerable<Vehicle>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Vehicles
                .Include(v => v.Owner)
                .Where(v => v.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetAllAsync()
        {
            return await _context.Vehicles
                .Include(v => v.Owner)
                .ToListAsync();
        }

        public async Task AddAsync(Vehicle vehicle)
        {
            await _context.Vehicles.AddAsync(vehicle);
        }

        public void Delete(Vehicle vehicle)
        {
            _context.Vehicles.Remove(vehicle);
        }

        public void Update(Vehicle vehicle)
        {
            _context.Entry(vehicle).State = EntityState.Modified;
        }

        public async Task<IEnumerable<ServiceRequest>> GetServiceHistoryAsync(Guid vehicleId)
        {
            return await _context.ServiceRequests
                .Include(sr => sr.AssignedProvider)
                .Where(sr => sr.VehicleId == vehicleId)
                .OrderByDescending(sr => sr.RequestedAt)
                .ToListAsync();
        }
    }
} 