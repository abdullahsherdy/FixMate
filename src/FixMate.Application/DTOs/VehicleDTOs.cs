using System;

namespace FixMate.Application.DTOs
{
    public class VehicleDto
    {
        public Guid Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string LicensePlate { get; set; }
        public Guid OwnerId { get; set; }
        public string OwnerName { get; set; }
    }

    public class CreateVehicleDto
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string LicensePlate { get; set; }
        public Guid OwnerId { get; set; }
    }

    public class UpdateVehicleDto
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
    }
} 