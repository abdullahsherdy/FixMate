using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
namespace FixMate.Domain.Entities
{
    public class UserRole
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        [JsonIgnore]
        public User User { get; set; }
        [ForeignKey("Id")]
        [JsonIgnore]
        public Role Role { get; set; }
    }
} 