using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using FixMate.Application.Interfaces.Services;
using FixMate.Application.Interfaces.Persistence;
using FixMate.Domain.Entities;
using FixMate.Domain.Enums;
using FixMate.Application.DTOs;
using Microsoft.Extensions.Logging;

namespace FixMate.Application.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IServiceProviderRepository _serviceProviderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ServiceRequestService> _logger;

        public ServiceRequestService(
            IServiceRequestRepository serviceRequestRepository,
            IVehicleRepository vehicleRepository,
            IServiceProviderRepository serviceProviderRepository,
            IUnitOfWork unitOfWork,
            ILogger<ServiceRequestService> logger)
        {
            _serviceRequestRepository = serviceRequestRepository ?? throw new ArgumentNullException(nameof(serviceRequestRepository));
            _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
            _serviceProviderRepository = serviceProviderRepository ?? throw new ArgumentNullException(nameof(serviceProviderRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ServiceRequestDto> CreateServiceRequestAsync(CreateServiceRequestDto requestDto)
        {
            if (requestDto == null)
                throw new ArgumentNullException(nameof(requestDto));

            try
            {
                // Validate vehicle exists
                var vehicle = await _vehicleRepository.GetByIdAsync(requestDto.VehicleId);
                if (vehicle == null)
                    throw new ArgumentException("Vehicle not found", nameof(requestDto.VehicleId));

                var request = new ServiceRequest
                {
                    VehicleId = requestDto.VehicleId,
                    ServiceType = requestDto.ServiceType,
                    Notes = requestDto.Notes,
                    Status = ServiceStatus.Pending,
                    RequestedAt = DateTime.UtcNow
                };

                await _serviceRequestRepository.AddAsync(request);
                await _unitOfWork.SaveChangesAsync();
                
                return MapToDto(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating service request for vehicle {VehicleId}", requestDto.VehicleId);
                throw;
            }
        }

        public async Task<ServiceRequestDto> GetServiceRequestByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid service request ID", nameof(id));

            try
            {
                var request = await _serviceRequestRepository.GetByIdAsync(id);
                if (request == null)
                    return null;

                return MapToDto(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting service request with ID {RequestId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<ServiceRequestDto>> GetServiceRequestsByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Invalid user ID", nameof(userId));

            try
            {
                var requests = await _serviceRequestRepository.GetByUserIdAsync(userId);
                return requests.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting service requests for user {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<ServiceRequestDto>> GetServiceRequestsByMechanicIdAsync(Guid mechanicId)
        {
            if (mechanicId == Guid.Empty)
                throw new ArgumentException("Invalid mechanic ID", nameof(mechanicId));

            try
            {
                var requests = await _serviceRequestRepository.GetByMechanicIdAsync(mechanicId);
                return requests.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting service requests for mechanic {MechanicId}", mechanicId);
                throw;
            }
        }

        public async Task<ServiceRequestDto> UpdateServiceStatusAsync(Guid requestId, UpdateServiceStatusDto statusDto)
        {
            if (requestId == Guid.Empty)
                throw new ArgumentException("Invalid service request ID", nameof(requestId));
            if (statusDto == null)
                throw new ArgumentNullException(nameof(statusDto));

            try
            {
                var request = await _serviceRequestRepository.GetByIdAsync(requestId);
                if (request == null)
                    throw new ArgumentException("Service request not found", nameof(requestId));

                request.Status = statusDto.Status;
                if (statusDto.Status == ServiceStatus.Completed)
                {
                    request.CompletedAt = DateTime.UtcNow;
                }

                _serviceRequestRepository.Update(request);
                await _unitOfWork.SaveChangesAsync();
                
                return MapToDto(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating status for request {RequestId}", requestId);
                throw;
            }
        }

        public async Task<ServiceRequestDto> AssignServiceProviderAsync(Guid requestId, AssignServiceProviderDto assignDto)
        {
            if (requestId == Guid.Empty)
                throw new ArgumentException("Invalid service request ID", nameof(requestId));
            if (assignDto == null)
                throw new ArgumentNullException(nameof(assignDto));

            try
            {
                var request = await _serviceRequestRepository.GetByIdAsync(requestId);
                if (request == null)
                    throw new ArgumentException("Service request not found", nameof(requestId));

                var provider = await _serviceProviderRepository.GetByIdAsync(assignDto.AssignedProviderId);
                if (provider == null)
                    throw new ArgumentException("Service provider not found", nameof(assignDto.AssignedProviderId));

                request.AssignedProviderId = assignDto.AssignedProviderId;
                request.Status = ServiceStatus.Assigned;

                _serviceRequestRepository.Update(request);
                await _unitOfWork.SaveChangesAsync();
                
                return MapToDto(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while assigning provider {ProviderId} to request {RequestId}", 
                    assignDto.AssignedProviderId, requestId);
                throw;
            }
        }

        public async Task<ServiceRequestDto> UpdateServiceRequestAsync(Guid id, UpdateServiceRequestDto requestDto)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid service request ID", nameof(id));
            if (requestDto == null)
                throw new ArgumentNullException(nameof(requestDto));

            try
            {
                var existingRequest = await _serviceRequestRepository.GetByIdAsync(id);
                if (existingRequest == null)
                    throw new ArgumentException("Service request not found", nameof(id));

                existingRequest.ServiceType = requestDto.ServiceType;
                existingRequest.Notes = requestDto.Notes;

                _serviceRequestRepository.Update(existingRequest);
                await _unitOfWork.SaveChangesAsync();
                
                return MapToDto(existingRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating service request with ID {RequestId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteServiceRequestAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid service request ID", nameof(id));

            try
            {
                var request = await _serviceRequestRepository.GetByIdAsync(id);
                if (request == null)
                    return false;

                _serviceRequestRepository.Delete(request);
                await _unitOfWork.SaveChangesAsync();
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting service request with ID {RequestId}", id);
                throw;
            }
        }

        private static ServiceRequestDto MapToDto(ServiceRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return new ServiceRequestDto
            {
                Id = request.Id,
                VehicleId = request.VehicleId,
                ServiceType = request.ServiceType,
                Status = request.Status,
                Notes = request.Notes ?? throw new InvalidOperationException("Service request notes cannot be null"),
                RequestedAt = request.RequestedAt,
                CompletedAt = request.CompletedAt,
                AssignedProviderId = request.AssignedProviderId,
                AssignedProviderName = request.AssignedProvider?.FullName
            };
        }
    }
} 