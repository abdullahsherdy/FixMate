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
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
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

        public async Task<UserDto> ValidateUserAsync(LoginRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            ValidateRequest(request);

            try
            {
                var user = await _userRepository.GetByEmailAsync(request.Email);
                if (user == null)
                    return null;

                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
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

                // Hash the password with BCrypt
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                var user = new User
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    PasswordHash = passwordHash,
                    PhoneNumber = request.PhoneNumber,
                    Role = new Role { Name= request.Role},
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

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
} 