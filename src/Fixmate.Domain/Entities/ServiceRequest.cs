using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FixMate.Domain.Enums;

namespace FixMate.Domain.Entities
{
    public class ServiceRequest
    {
        public Guid Id { get; set; }
        public Guid VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }

        public Guid ServiceTypeId { get; set; }
        public ServiceType ServiceType { get; set; }

        public Guid? AssignedProviderId { get; set; }
        public ServiceProvider AssignedProvider { get; set; }

        public ServiceStatus Status { get; set; }
        public string Notes { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

}
