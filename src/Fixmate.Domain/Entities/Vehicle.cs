using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FixMate.Domain.ValueObjects;
namespace FixMate.Domain.Entities
{
    public class Vehicle
    {
        public Guid Id { get; set; }
        public string Make { get; set; }          // Toyota, Honda
        public string Model { get; set; }         // Camry, Civic
        public string Type { get; set; }          // Car, Bike
        public LicensePlate LicensePlate { get; set; }
        public Guid UserId { get; set; }
        public User Owner { get; set; }
    }

}
