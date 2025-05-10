using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using FixMate.Application.Interfaces.Services;
using FixMate.Application.Interfaces.Persistence;
using FixMate.Domain.Entities;
using FixMate.Domain.Enums;
using FixMate.Domain.ValueObjects;
using FixMate.Application.DTOs;
using Microsoft.Extensions.Logging;

namespace FixMate.Application.Services
{
    public class ServiceProviderService : IServiceProviderService
    {
        private readonly IServiceProviderRepository _serviceProviderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ServiceProviderService> _logger;

        public ServiceProviderService(
            IServiceProviderRepository serviceProviderRepository,
            IUnitOfWork unitOfWork,
            ILogger<ServiceProviderService> logger)
        {
            _serviceProviderRepository = serviceProviderRepository ?? throw new ArgumentNullException(nameof(serviceProviderRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ServiceProviderDto> CreateServiceProviderAsync(CreateServiceProviderDto providerDto)
        {
            if (providerDto == null)
                throw new ArgumentNullException(nameof(providerDto));

            try
            {
                // Check if provider with same email already exists
                var existingProvider = await _serviceProviderRepository.GetByEmailAsync(providerDto.Email);
                if (existingProvider != null)
                    throw new ArgumentException("A service provider with this email already exists", nameof(providerDto.Email));

                var provider = new ServiceProvider
                {
                    FullName = providerDto.FullName,
                    Email = new Email(providerDto.Email),
                    PhoneNumber = providerDto.PhoneNumber,
                    Specialization = providerDto.Specialization,
                    IsAvailable = true,
                    AssignedRequests = new List<ServiceRequest>()
                };

                await _serviceProviderRepository.AddAsync(provider);
                await _unitOfWork.SaveChangesAsync();
                
                return MapToDto(provider);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating service provider with email {Email}", providerDto.Email);
                throw;
            }
        }

        public async Task<ServiceProviderDto> GetServiceProviderByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid service provider ID", nameof(id));

            try
            {
                var provider = await _serviceProviderRepository.GetByIdAsync(id);
                if (provider == null)
                    return null;

                return MapToDto(provider);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting service provider with ID {ProviderId}", id);
                throw;
            }
        }

        public async Task<ServiceProviderDto> GetServiceProviderByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            try
            {
                var provider = await _serviceProviderRepository.GetByEmailAsync(email);
                if (provider == null)
                    return null;

                return MapToDto(provider);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting service provider with email {Email}", email);
                throw;
            }
        }

        public async Task<IEnumerable<ServiceProviderDto>> GetServiceProvidersBySpecializationAsync(Specialization specialization)
        {
            try
            {
                var providers = await _serviceProviderRepository.GetBySpecializationAsync(specialization);
                return providers.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting service providers with specialization {Specialization}", specialization);
                throw;
            }
        }

        public async Task<IEnumerable<ServiceProviderDto>> GetAllServiceProvidersAsync()
        {
            try
            {
                var providers = await _serviceProviderRepository.GetAllAsync();
                return providers.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all service providers");
                throw;
            }
        }

        public async Task<ServiceProviderDto> UpdateServiceProviderAsync(Guid id, UpdateServiceProviderDto providerDto)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid service provider ID", nameof(id));
            if (providerDto == null)
                throw new ArgumentNullException(nameof(providerDto));

            try
            {
                var existingProvider = await _serviceProviderRepository.GetByIdAsync(id);
                if (existingProvider == null)
                    throw new ArgumentException("Service provider not found", nameof(id));

                existingProvider.FullName = providerDto.FullName;
                existingProvider.PhoneNumber = providerDto.PhoneNumber;
                existingProvider.Specialization = providerDto.Specialization;

                _serviceProviderRepository.Update(existingProvider);
                await _unitOfWork.SaveChangesAsync();
                
                return MapToDto(existingProvider);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating service provider with ID {ProviderId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteServiceProviderAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid service provider ID", nameof(id));

            try
            {
                var provider = await _serviceProviderRepository.GetByIdAsync(id);
                if (provider == null)
                    return false;

                _serviceProviderRepository.Delete(provider);
                await _unitOfWork.SaveChangesAsync();
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting service provider with ID {ProviderId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<ServiceRequestDto>> GetAssignedServiceRequestsAsync(Guid providerId)
        {
            if (providerId == Guid.Empty)
                throw new ArgumentException("Invalid service provider ID", nameof(providerId));

            try
            {
                var requests = await _serviceProviderRepository.GetAssignedRequests(providerId);
                /// Manual Mapping 
                return requests.Select(sr => new ServiceRequestDto
                {
                    Id = sr.Id,
                    VehicleId = sr.VehicleId,
                    ServiceType = sr.ServiceType,
                    Status = sr.Status,
                    Notes = sr.Notes,
                    RequestedAt = sr.RequestedAt,
                    CompletedAt = sr.CompletedAt,
                    AssignedProviderId = sr.AssignedProviderId,
                    AssignedProviderName = sr.AssignedProvider?.FullName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting assigned requests for provider {ProviderId}", providerId);
                throw;
            }
        }

        public async Task<ServiceProviderDto> UpdateAvailabilityAsync(Guid id, UpdateAvailabilityDto availabilityDto)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid service provider ID", nameof(id));
            if (availabilityDto == null)
                throw new ArgumentNullException(nameof(availabilityDto));

            try
            {
                var provider = await _serviceProviderRepository.GetByIdAsync(id);
                if (provider == null)
                    throw new ArgumentException("Service provider not found", nameof(id));

                provider.IsAvailable = availabilityDto.IsAvailable;

                _serviceProviderRepository.Update(provider);
                await _unitOfWork.SaveChangesAsync();
                
                return MapToDto(provider);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating availability for provider {ProviderId}", id);
                throw;
            }
        }



        private static ServiceProviderDto MapToDto(ServiceProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            return new ServiceProviderDto
            {
                Id = provider.Id,
                FullName = provider.FullName ?? throw new InvalidOperationException("Service provider full name cannot be null"),
                Email = provider.Email?.Value ?? throw new InvalidOperationException("Service provider email cannot be null"),
                PhoneNumber = provider.PhoneNumber ?? throw new InvalidOperationException("Service provider phone number cannot be null"),
                Specialization = provider.Specialization,
                IsAvailable = provider.IsAvailable
            };
        }



    }
} 