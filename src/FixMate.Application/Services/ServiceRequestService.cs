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
                // Verify vehicle exists
                var vehicle = await _vehicleRepository.GetByIdAsync(requestDto.VehicleId);
                if (vehicle == null)
                    throw new ArgumentException("Vehicle not found", nameof(requestDto.VehicleId));

                var request = new ServiceRequest
                {
                    VehicleId = requestDto.VehicleId,
                    ServiceType = requestDto.ServiceType,
                    Status = ServiceStatus.Pending,
                    Notes = requestDto.Notes,
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

        public async Task<IEnumerable<ServiceRequestDto>> GetServiceRequestsByVehicleIdAsync(Guid vehicleId)
        {
            if (vehicleId == Guid.Empty)
                throw new ArgumentException("Invalid vehicle ID", nameof(vehicleId));

            try
            {
                var requests = await _serviceRequestRepository.GetByVehicleIdAsync(vehicleId);
                return requests.Select(r => MapToDto(r));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting service requests for vehicle {VehicleId}", vehicleId);
                throw;
            }
        }

        public async Task<IEnumerable<ServiceRequestDto>> GetServiceRequestsByProviderIdAsync(Guid providerId)
        {
            if (providerId == Guid.Empty)
                throw new ArgumentException("Invalid service provider ID", nameof(providerId));

            try
            {
                var requests = await _serviceRequestRepository.GetByProviderIdAsync(providerId);
                return requests.Select(r => MapToDto(r));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting service requests for provider {ProviderId}", providerId);
                throw;
            }
        }

        public async Task<ServiceRequestDto> UpdateServiceRequestStatusAsync(Guid id, ServiceStatus status)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid service request ID", nameof(id));

            try
            {
                var request = await _serviceRequestRepository.GetByIdAsync(id);
                if (request == null)
                    throw new ArgumentException("Service request not found", nameof(id));

                request.Status = status;
                if (status == ServiceStatus.Completed)
                    request.CompletedAt = DateTime.UtcNow;

                _serviceRequestRepository.Update(request);
                await _unitOfWork.SaveChangesAsync();
                
                return MapToDto(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating status for service request {RequestId}", id);
                throw;
            }
        }

        public async Task<ServiceRequestDto> AssignServiceProviderAsync(Guid requestId, Guid providerId)
        {
            if (requestId == Guid.Empty)
                throw new ArgumentException("Invalid service request ID", nameof(requestId));
            if (providerId == Guid.Empty)
                throw new ArgumentException("Invalid service provider ID", nameof(providerId));

            try
            {
                var request = await _serviceRequestRepository.GetByIdAsync(requestId);
                if (request == null)
                    throw new ArgumentException("Service request not found", nameof(requestId));

                var provider = await _serviceProviderRepository.GetByIdAsync(providerId);
                if (provider == null)
                    throw new ArgumentException("Service provider not found", nameof(providerId));

                if (!provider.IsAvailable)
                    throw new InvalidOperationException("Service provider is not available");

                request.AssignedProviderId = providerId;
                request.Status = ServiceStatus.InProgress;

                _serviceRequestRepository.Update(request);
                await _unitOfWork.SaveChangesAsync();
                
                return MapToDto(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while assigning provider {ProviderId} to request {RequestId}", providerId, requestId);
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

        public async Task<IEnumerable<ServiceRequestDto>> GetServiceRequestsByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Invalid user ID", nameof(userId));

            try
            {
                var requests = await _serviceRequestRepository.GetByOwnerIdAsync(userId);
                return requests.Select(r => MapToDto(r));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting service requests for user {UserId}", userId);
                throw;
            }
        }

        public async Task<ServiceRequestDto> UpdateServiceStatusAsync(Guid requestId, UpdateServiceRequestStatusDto statusDto)
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
                    request.CompletedAt = DateTime.UtcNow;

                _serviceRequestRepository.Update(request);
                await _unitOfWork.SaveChangesAsync();
                
                return MapToDto(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating status for service request {RequestId}", requestId);
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

                var provider = await _serviceProviderRepository.GetByIdAsync(assignDto.ProviderId);
                if (provider == null)
                    throw new ArgumentException("Service provider not found", nameof(assignDto.ProviderId));

                if (!provider.IsAvailable)
                    throw new InvalidOperationException("Service provider is not available");

                request.AssignedProviderId = assignDto.ProviderId;
                request.Status = ServiceStatus.InProgress;

                _serviceRequestRepository.Update(request);
                await _unitOfWork.SaveChangesAsync();
                
                return MapToDto(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while assigning provider {ProviderId} to request {RequestId}", assignDto.ProviderId, requestId);
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
                Notes = request.Notes,
                RequestedAt = request.RequestedAt,
                CompletedAt = request.CompletedAt,
                AssignedProviderId = request.AssignedProviderId,
                AssignedProviderName = request.AssignedProvider?.FullName
            };
        }
    }
} 