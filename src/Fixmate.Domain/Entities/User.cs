using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using FixMate.Domain.ValueObjects;
using FixMate.Domain.Enums;

namespace FixMate.Domain.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(20)]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public Role Role { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? LastLoginAt { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [JsonIgnore]
        public List<UserRole> UserRoles { get; set; } = new List<UserRole>();

        public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public virtual ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
    }
} 