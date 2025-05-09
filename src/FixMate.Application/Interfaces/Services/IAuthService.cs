using System;
using System.Threading.Tasks;
using FixMate.Application.DTOs;
using FixMate.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace FixMate.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<UserDto> ValidateUserAsync(string email, string password);
        Task<UserDto> RegisterUserAsync(RegisterRequest request);
        Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
        Task<UserDto> UpdateUserAsync(Guid userId, UpdateUserRequest request);
        Task<UserDto> GetUserByIdAsync(Guid userId);
    }
}