using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FixMate.Application.DTOs;

namespace FixMate.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserDto> CreateUserAsync(CreateUserDto userDto);
        Task<UserDto> GetUserByIdAsync(Guid id);
        Task<UserDto> GetUserByEmailAsync(string email);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto userDto);
        Task<bool> DeleteUserAsync(Guid id);
        Task<IEnumerable<VehicleDto>> GetUserVehiclesAsync(Guid userId);
    }
} 