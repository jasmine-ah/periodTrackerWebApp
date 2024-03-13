using System;
using System.ComponentModel.DataAnnotations;

namespace periodTracker.Models.ViewModels
{
	public class ResetPasswordViewModel
	{
        public string UserId { get; set; }

        public string Token { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}

