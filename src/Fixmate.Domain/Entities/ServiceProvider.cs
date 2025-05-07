using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FixMate.Domain.Enums;

namespace FixMate.Domain.Entities
{
    public class ServiceProvider
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public Specialization Specialization { get; set; }
        public bool IsAvailable { get; set; }

        public ICollection<ServiceRequest> AssignedRequests { get; set; }
    }

}
