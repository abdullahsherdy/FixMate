using System;
using System.ComponentModel.DataAnnotations;
using FixMate.Domain.Entities;

namespace FixMate.Application.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public Role Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; }
    }

    //public class RegisterRequest
    //{
    //    [Required]
    //    [StringLength(100)]
    //    public string FullName { get; set; }

    //    [Required]
    //    [EmailAddress]
    //    [StringLength(100)]
    //    public string Email { get; set; }

    //    [Required]
    //    [StringLength(100, MinimumLength = 6)]
    //    public string Password { get; set; }

    //    [Required]
    //    [Phone]
    //    [StringLength(20)]
    //    public string PhoneNumber { get; set; }

    //    [Required]
    //    public Role Role { get; set; }
    //}

    //public class LoginRequest
    //{
    //    [Required]
    //    [EmailAddress]
    //    public string Email { get; set; }

    //    [Required]
    //    public string Password { get; set; }
    //}

    //public class ChangePasswordRequest
    //{
    //    [Required]
    //    public string CurrentPassword { get; set; }

    //    [Required]
    //    [StringLength(100, MinimumLength = 6)]
    //    public string NewPassword { get; set; }
    //}

    public class UpdateUserRequest
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; }
    }
} 