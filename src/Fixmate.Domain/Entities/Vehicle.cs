using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FixMate.Domain.Entities
{
    public class Vehicle
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(50)]
        public string Make { get; set; }

        [Required]
        [StringLength(50)]
        public string Model { get; set; }

        [Required]
        [Range(1900, 2100)]
        public int Year { get; set; }

        [Required]
        [StringLength(20)]
        public string LicensePlate { get; set; }

        [Required]
        [ForeignKey("Owner")]
        public Guid OwnerId { get; set; }
        public virtual User Owner { get; set; }

        // Navigation properties
        public virtual ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
    }
}
