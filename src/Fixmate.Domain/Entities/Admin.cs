using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixMate.Domain.Entities
{
    public class Admin
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        // Later: Audit logs, permissions, etc.
    }

}
