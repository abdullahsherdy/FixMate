using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FixMate.Domain.ValueObjects;
namespace FixMate.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public Email Email { get; set; }
        public ICollection<Vehicle> Vehicles { get; set; }
    }

}
