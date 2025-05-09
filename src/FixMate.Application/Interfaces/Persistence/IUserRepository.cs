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
        Task<IEnumerable<User>> GetAllAsync();
        Task AddAsync(User user);
        void Delete(User user);
        void Update(User user);
        Task<IEnumerable<Vehicle>> GetUserVehiclesAsync(Guid userId);
    }
} 