using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using FixMate.Application.Interfaces.Services;
using FixMate.Application.Interfaces.Persistence;
using FixMate.Domain.Entities;
using FixMate.Application.DTOs;
using Microsoft.Extensions.Logging;

namespace FixMate.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto userDto)
        {
            if (userDto == null)
                throw new ArgumentNullException(nameof(userDto));

            try
            {
                // Check if user with same email exists
                var existingUser = await _userRepository.GetByEmailAsync(userDto.Email);
                if (existingUser != null)
                    throw new InvalidOperationException("User with this email already exists");

                var user = new User
                {
                    Email = userDto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    PhoneNumber = userDto.PhoneNumber,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    UserRoles = new List<UserRole>()
                };

                await _userRepository.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();
                
                return MapToDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating user with email {Email}", userDto.Email);
                throw;
            }
        }

        public async Task<UserDto> GetUserByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid user ID", nameof(id));

            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                    return null;

                return MapToDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user with ID {UserId}", id);
                throw;
            }
        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            try
            {
                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                    return null;

                return MapToDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user with email {Email}", email);
                throw;
            }
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                return users.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all users");
                throw;
            }
        }

        public async Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto userDto)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid user ID", nameof(id));
            if (userDto == null)
                throw new ArgumentNullException(nameof(userDto));

            try
            {
                var existingUser = await _userRepository.GetByIdAsync(id);
                if (existingUser == null)
                    throw new ArgumentException("User not found", nameof(id));

                existingUser.FirstName = userDto.FirstName;
                existingUser.LastName = userDto.LastName;
                existingUser.PhoneNumber = userDto.PhoneNumber;

                _userRepository.Update(existingUser);
                await _unitOfWork.SaveChangesAsync();
                
                return MapToDto(existingUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user with ID {UserId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid user ID", nameof(id));

            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                    return false;

                _userRepository.Delete(user);
                await _unitOfWork.SaveChangesAsync();
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user with ID {UserId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<VehicleDto>> GetUserVehiclesAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Invalid user ID", nameof(userId));

            try
            {
                var vehicles = await _userRepository.GetUserVehiclesAsync(userId);
                return vehicles.Select(v => new VehicleDto
                {
                    Id = v.Id,
                    Make = v.Make,
                    Model = v.Model,
                    Type = v.Type,
                    LicensePlate = v.LicensePlate.Value,
                    UserId = v.UserId,
                    OwnerName = $"{v.Owner.FirstName} {v.Owner.LastName}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting vehicles for user with ID {UserId}", userId);
                throw;
            }
        }

        private static UserDto MapToDto(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email ?? throw new InvalidOperationException("User email cannot be null"),
                FirstName = user.FirstName ?? throw new InvalidOperationException("User first name cannot be null"),
                LastName = user.LastName ?? throw new InvalidOperationException("User last name cannot be null"),
                PhoneNumber = user.PhoneNumber ?? throw new InvalidOperationException("User phone number cannot be null"),
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                IsActive = user.IsActive,
                Roles = user.UserRoles?.Select(ur => ur.Role.Name).ToList() ?? new List<string>()
            };
        }
    }
} 