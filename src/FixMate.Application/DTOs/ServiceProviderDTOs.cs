using System;
using FixMate.Domain.Enums;

namespace FixMate.Application.DTOs
{
    public class ServiceProviderDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public Specialization Specialization { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class CreateServiceProviderDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public Specialization Specialization { get; set; }
    }

    public class UpdateServiceProviderDto
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public Specialization Specialization { get; set; }
    }

    public class UpdateAvailabilityDto
    {
        public bool IsAvailable { get; set; }
    }
} 