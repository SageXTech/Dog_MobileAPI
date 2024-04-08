using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DogAPI.Models
{
    public class LoginModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class AuthView
    {
        public string Token { get; set; }
        public int Id { get; set; }
    }

    public class SignUp
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public enum DisplayRole
    {
        Admin = 2,
        User = 3,
    }

    public class NewPassword
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        public string Confirm_Password { get; set; }
    }

    public class ChangePasswordModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }

    public class ForgotPasswordVM
    {
        [Required]
        public string Email { get; set; }
    }

    public class ConfirmToken
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string OTP { get; set; }
    }
    public class GuestModel
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string DeviceId { get; set; }
        [Required]
        public string Name { get; set; }
    }
}