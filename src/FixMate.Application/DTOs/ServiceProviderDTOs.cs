using System;
using System.ComponentModel.DataAnnotations;
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
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [Required]
        public Specialization Specialization { get; set; }
    }

    public class UpdateServiceProviderDto
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [Required]
        public Specialization Specialization { get; set; }
    }

    public class UpdateAvailabilityDto
    {
        [Required]
        public bool IsAvailable { get; set; }
    }
} 