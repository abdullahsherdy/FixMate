using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FixMate.Domain.ValueObjects
{
    public class Email : IEquatable<Email>
    {
        public string Value { get; }

        public Email(string value)
        {
            Value = value;
        }

        public static Email Create(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.");

            if (!Regex.IsMatch(email, @"^\S+@\S+\.\S+$"))
                throw new ArgumentException("Invalid email format.");

            return new Email(email.ToLowerInvariant());
        }

        public override string ToString() => Value;

        public override bool Equals(object obj) => Equals(obj as Email);

        public bool Equals(Email other) => other != null && Value == other.Value;

        public override int GetHashCode() => Value.GetHashCode();

        public static implicit operator string(Email email) => email.Value;
    }

}
