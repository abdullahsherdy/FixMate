using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FixMate.Application.DTOs;
using FixMate.Domain.Enums;

namespace FixMate.Application.Interfaces.Services
{
    public interface IServiceRequestService
    {
        Task<ServiceRequestDto> CreateServiceRequestAsync(CreateServiceRequestDto requestDto); // User Book A Service for abike or a car 
        Task<ServiceRequestDto> GetServiceRequestByIdAsync(Guid id); // Id of SR 
        Task<IEnumerable<ServiceRequestDto>> GetServiceRequestsByUserIdAsync(Guid userId); // SR by Customer 
        Task<ServiceRequestDto> UpdateServiceStatusAsync(Guid requestId, UpdateServiceRequestStatusDto statusDto);
        Task<ServiceRequestDto> AssignServiceProviderAsync(Guid requestId, AssignServiceProviderDto assignDto);
        Task<ServiceRequestDto> UpdateServiceRequestAsync(Guid id, UpdateServiceRequestDto requestDto);
        Task<IEnumerable<ServiceRequestDto>> GetServiceRequestsByVehicleIdAsync(Guid vehicleId);
        Task<IEnumerable<ServiceRequestDto>> GetServiceRequestsByProviderIdAsync(Guid providerId);
        Task<ServiceRequestDto> UpdateServiceRequestStatusAsync(Guid id, ServiceStatus status);
        Task<ServiceRequestDto> AssignServiceProviderAsync(Guid requestId, Guid providerId);
        Task<bool> DeleteServiceRequestAsync(Guid id);

    
    }
} 