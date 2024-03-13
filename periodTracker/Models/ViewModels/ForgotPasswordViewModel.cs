using System;
using System.ComponentModel.DataAnnotations;

namespace periodTracker.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

}

