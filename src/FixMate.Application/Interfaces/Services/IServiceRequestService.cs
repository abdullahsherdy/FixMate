using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FixMate.Application.DTOs;

namespace FixMate.Application.Interfaces.Services
{
    public interface IServiceRequestService
    {
        Task<ServiceRequestDto> CreateServiceRequestAsync(CreateServiceRequestDto requestDto);
        Task<ServiceRequestDto> GetServiceRequestByIdAsync(Guid id);
        Task<IEnumerable<ServiceRequestDto>> GetServiceRequestsByUserIdAsync(Guid userId);
        Task<ServiceRequestDto> UpdateServiceStatusAsync(Guid requestId, UpdateServiceRequestStatusDto statusDto);
        Task<ServiceRequestDto> AssignServiceProviderAsync(Guid requestId, AssignServiceProviderDto assignDto);
        Task<ServiceRequestDto> UpdateServiceRequestAsync(Guid id, UpdateServiceRequestDto requestDto);
        Task<bool> DeleteServiceRequestAsync(Guid id);
    }
} 