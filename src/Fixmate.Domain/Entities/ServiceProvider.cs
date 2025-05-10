using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FixMate.Domain.Enums;

namespace FixMate.Domain.Entities
{
    /// <summary>
    ///  User, Customer, Vehicle 
    ///  user -> Customer -> ServiceRequest 
    /// </summary>
    public class ServiceProvider
    {
        /// <summary>
        ///  Global Unifed Id -> generate new id unique 
        /// </summary>
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required] // not null 
        [MaxLength(100)]  // nvarchar(100) 
        public string FullName { get; set; }

        [EmailAddress]  
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        // As A Default Value our platform is for Mehanics 
        public Specialization Specialization { get; set; } = Specialization.Mechanic; 
        // Every Provider is Available by default 
        public bool IsAvailable { get; set; } = true;

        [Required]
        public string PasswordHash { get; set; }

        /// <summary>
        ///  Relations -> Fk 
        ///  EF -> navigational Properties 
        ///  ServiceProvide -> user 
        ///  user -> ask  multi serviceprovider 
        ///  many to many 
        ///  branch -> AssignedRequests -> ServiceRequest 
        ///  Ef runtime -> understand  
        ///  one to one -> nav 
        ///  one to many -> igonre 
        ///  many to many -> required 
        /// </summary>
        public List<ServiceRequest> AssignedRequests { get; set; } = new List<ServiceRequest>();
    }

}
