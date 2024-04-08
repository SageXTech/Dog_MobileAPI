using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DogAPI.Areas.Admin.Models
{
    public class LoginModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Email Address")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password")]
        public string Confirm_Password { get; set; }

        [Required]
        [Display(Name = "Role")]
        public int? Role { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
    public class RoleView
    {
        public int Id { get; set; }
        [Required]
        public string Role { get; set; }
        [Required]
        [Display(Name = "Email Address")]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class ForgotPasswordModel
    {
        [Required]
        [Display(Name = "Email Address")]
        [EmailAddress]
        public string Email { get; set; }
    }
    public class TokenVerifyModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        [Required]
        public string Token { get; set; }
    }
    public class NewPassword
    {
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password")]
        public string Confirm_Password { get; set; }
    }

    public class ChangePasswordModel
    {
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginView
    {
        [Required]
        [Display(Name = "Email Address")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public enum UserRole
    {
        SperAdmin=1,
        Admin=2,
        User =3,
    }
    public enum DisplayRole
    {
        Admin = 2,
        User = 3,
    }
}