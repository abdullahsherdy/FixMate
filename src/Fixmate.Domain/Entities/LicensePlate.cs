using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FixMate.Domain.ValueObjects
{
    public class LicensePlate : IEquatable<LicensePlate>
    {
    
        public string Value { get; private set; }
        public LicensePlate(string value)
        {
            Value = value;
        }

        public static LicensePlate Create(string plate)
        {
            if (string.IsNullOrWhiteSpace(plate))
                throw new ArgumentException("License plate is required.");

            if (!Regex.IsMatch(plate, @"^[A-Z0-9\-]{4,10}$"))
                throw new ArgumentException("Invalid license plate format.");

            return new LicensePlate(plate.ToUpperInvariant());
        }

        public override string ToString() => Value;

        public override bool Equals(object obj) => Equals(obj as LicensePlate);

        public bool Equals(LicensePlate other) => other != null && Value == other.Value;

        public override int GetHashCode() => Value.GetHashCode();

        public static implicit operator string(LicensePlate plate) => plate.Value;
    }

}
