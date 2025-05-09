using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FixMate.Application.DTOs;
using FixMate.Domain.Enums;

namespace FixMate.Application.Interfaces.Services
{
    public interface IServiceProviderService
    {
        Task<ServiceProviderDto> CreateServiceProviderAsync(CreateServiceProviderDto providerDto);
        Task<ServiceProviderDto> GetServiceProviderByIdAsync(Guid id);
        Task<ServiceProviderDto> GetServiceProviderByEmailAsync(string email);
        Task<IEnumerable<ServiceProviderDto>> GetServiceProvidersBySpecializationAsync(Specialization specialization);
        Task<IEnumerable<ServiceProviderDto>> GetAllServiceProvidersAsync();
        Task<ServiceProviderDto> UpdateServiceProviderAsync(Guid id, UpdateServiceProviderDto providerDto);
        Task<bool> DeleteServiceProviderAsync(Guid id);
        Task<IEnumerable<ServiceRequestDto>> GetAssignedServiceRequestsAsync(Guid providerId);
        Task<bool> UpdateAvailabilityAsync(Guid providerId, UpdateAvailabilityDto availabilityDto);
    }
} 