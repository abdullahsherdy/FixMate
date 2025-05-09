using System;
using System.Collections.Generic;

namespace Fixmate.Domain.Entities
{
    public class Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
} 