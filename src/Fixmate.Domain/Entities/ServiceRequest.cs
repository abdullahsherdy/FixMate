using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FixMate.Domain.Enums;

namespace FixMate.Domain.Entities
{
    public class ServiceRequest
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [ForeignKey("Vehicle")]
        public Guid VehicleId { get; set; }
        public virtual Vehicle Vehicle { get; set; }

        public Guid UserId { get; set; } // Assuming this is the ID of the user who made the request
        public virtual User User { get; set; } // Assuming this is the user entity

        public virtual ServiceProvider ServiceProvider { get; set; }

        public Guid ServiceProviderId { get; set; } // Assuming this is the ID of the service provider assigned to the request

        [Required]
        public ServiceType ServiceType { get; set; }

        [Required]
        public ServiceStatus Status { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        [Required]
        public DateTime RequestedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        [ForeignKey("AssignedProvider")]
        public Guid? AssignedProviderId { get; set; }
        public virtual ServiceProvider AssignedProvider { get; set; }
    }
}
