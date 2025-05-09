using System;
using FixMate.Domain.Enums;

namespace FixMate.Application.DTOs
{
    public class ServiceRequestDto
    {
        public Guid Id { get; set; }
        public Guid VehicleId { get; set; }
        public string VehicleInfo { get; set; }
        public ServiceType ServiceType { get; set; }
        public Guid? AssignedProviderId { get; set; }
        public string AssignedProviderName { get; set; }
        public ServiceStatus Status { get; set; }
        public string Notes { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    public class CreateServiceRequestDto
    {
        public Guid VehicleId { get; set; }
        public ServiceType ServiceType { get; set; }
        public string Notes { get; set; }
    }

    public class UpdateServiceRequestDto
    {
        public ServiceType ServiceType { get; set; }
        public string Notes { get; set; }
    }

    public class AssignServiceProviderDto
    {
        public Guid AssignedProviderId { get; set; }
    }

    public class UpdateServiceStatusDto
    {
        public ServiceStatus Status { get; set; }
    }
} 