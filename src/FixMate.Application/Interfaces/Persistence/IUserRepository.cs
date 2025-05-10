using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FixMate.Domain.Entities;

namespace FixMate.Application.Interfaces.Persistence
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid id);
        Task<User> GetByEmailAsync(string email);
        Task<IEnumerable<Vehicle>> GetUserVehiclesAsync(Guid userId);
        Task<IEnumerable<User>> GetAllAsync();
        Task AddAsync(User user);
        void Update(User user);
        void Delete(User user);
    }
} 