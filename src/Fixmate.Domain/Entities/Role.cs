using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FixMate.Domain.Entities
{
    public class Role
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        [JsonIgnore]
        public List<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
} 