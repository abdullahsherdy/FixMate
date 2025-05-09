using System;
using System.Threading.Tasks;
using FixMate.Application.DTOs;
using FixMate.Domain.Entities;

namespace FixMate.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<User> ValidateUserAsync(string email, string password);
        Task<User> RegisterUserAsync(RegisterRequest request);
        Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
    }
} 