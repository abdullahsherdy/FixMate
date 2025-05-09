using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FixMate.Application.DTOs;

namespace FixMate.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserDto> CreateUserAsync(UserDto userDto);
        Task<UserDto> GetUserByIdAsync(Guid id);
        Task<UserDto> GetUserByEmailAsync(string email);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        /// <summary>
        ///  Update ONly Name and  phone number 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userDto"></param>
        /// <returns></returns>
        Task<UserDto> UpdateUserAsync(Guid id, UpdateUserRequest userDto);
        Task<bool> DeleteUserAsync(Guid id);
        Task<IEnumerable<VehicleDto>> GetUserVehiclesAsync(Guid userId);
    }
} 