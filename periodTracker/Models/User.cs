using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace periodTracker.Models
{
    public class User:IdentityUser
    {
        //[Key]
        //public int Id { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50, ErrorMessage = "First Name cannot be longer than 50 characters.")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50, ErrorMessage = "Last Name cannot be longer than 50 characters.")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Age is required")]
        [Range(18, 100, ErrorMessage = "You must be between 18 and 100 years old.")]
        public int Age { get; set; }

        //[Required(ErrorMessage = "Username is required")]
        //[StringLength(50, ErrorMessage = "Username cannot be longer than 50 characters.")]
        //public string? Username { get; set; }

        //[Required(ErrorMessage = "Password is required")]
        //[StringLength(50, ErrorMessage = "Password cannot be longer than 50 characters.")]
        //public string? Password { get; set; }

        //[Required(ErrorMessage = "Email is required")]
        //[EmailAddress(ErrorMessage = "Invalid Email Address")]
        //public string? Email { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        [Required(ErrorMessage = "Date of Birth is required")]
        public DateTime DateOfBirth { get; set; }

     
    }
}
