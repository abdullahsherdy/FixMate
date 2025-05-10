using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FixMate.Domain.Enums;
using FixMate.Domain.ValueObjects;

namespace FixMate.Domain.Entities
{
    public class ServiceProvider
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        public Email Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        // As A Default Value our platform is for Mehanics 
        public Specialization Specialization { get; set; } = Specialization.Mechanic; 
        // Every Provider is Available by default 
        public bool IsAvailable { get; set; } = true;
        public List<ServiceRequest> AssignedRequests { get; set; } = new List<ServiceRequest>();
    }

}
