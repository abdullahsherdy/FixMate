using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FixMate.Domain.Enums;

namespace FixMate.Domain.Entities
{
    public class ServiceType
    {
        public Guid Id { get; set; }
        public string Name { get; set; } // Oil Change, Battery Check
        public Specialization RequiredSpecialization { get; set; }
    }

}
