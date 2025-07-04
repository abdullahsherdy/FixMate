using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using FixMate.Application.Interfaces.Services;
using FixMate.Application.Interfaces.Persistence;
using FixMate.Domain.Entities;
using FixMate.Domain.Enums;
using FixMate.Application.DTOs;
using BCrypt.Net;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace FixMate.Application.Services
{
    /// <summary>
    ///  This Service knows only about interface, which declared in application layer
    ///  in DI registration we register Repos as <Interface, Imp>, regardless of this we still respect clean arch principles 
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        // private readonly UserRepository _userRepository -> violate Clean Arch 
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UserDto> ValidateUserAsync(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                throw new ArgumentException("Email and password cannot be null or empty");
            
            var request = new LoginRequest
            {
                Email = email,
                Password = password
            };
           
            ValidateRequest(request);

            try
            {
                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                    return null;

                if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                    return null;

                user.LastLoginAt = DateTime.UtcNow;
                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return MapToDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while validating user with email {Email}", request.Email);
                throw;
            }
        }

        public async Task<UserDto> RegisterUserAsync(RegisterRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            ValidateRequest(request);

            try
            {
                var existingUser = await _userRepository.GetByEmailAsync(request.Email);
                if (existingUser != null)
                    throw new InvalidOperationException("User with this email already exists");

                // Hash the password with BCrypt, AES Encryption 
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password); 

                var user = new User
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    PasswordHash = passwordHash,
                    PhoneNumber = request.PhoneNumber,
                    Role = Role.Customer,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                await _userRepository.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return MapToDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering user with email {Email}", request.Email);
                throw;
            }
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Invalid user ID", nameof(userId));
           
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            ValidateRequest(request);

            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    return false;

                if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
                    return false;

                // Hash the new password
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while changing password for user {UserId}", userId);
                throw;
            }
        }

        public async Task<UserDto> UpdateUserAsync(Guid userId, UpdateUserRequest request)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Invalid user ID", nameof(userId));
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            ValidateRequest(request);

            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    throw new ArgumentException("User not found", nameof(userId));

                user.FullName = request.FullName;
                user.PhoneNumber = request.PhoneNumber;

                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return MapToDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user {UserId}", userId);
                throw;
            }
        }

        public async Task<UserDto> GetUserByIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Invalid user ID", nameof(userId));

            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    throw new ArgumentException("User not found", nameof(userId));

                return MapToDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user {UserId}", userId);
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
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                IsActive = user.IsActive
            };
        }

        /// <summary>
        ///  its purpose is to achieve a set of software engineering principles 
        ///  DRY (Don't Repeat Yourself) and KISS (Keep It Simple, Stupid).
        ///  Consistency in validation logic across the application.
        ///  Maintainability and readability of the code.
        ///  Error handling and logging.
        ///  performance and efficiency.
        /// </summary>
        /// <param name="request"></param>
        /// <exception cref="ValidationException"></exception>
        private static void ValidateRequest(object request)
        {
            var validationContext = new ValidationContext(request);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
            {
                var errors = string.Join(", ", validationResults.Select(r => r.ErrorMessage));
                throw new ValidationException($"Validation failed: {errors}");
            }
        }
    }
} 