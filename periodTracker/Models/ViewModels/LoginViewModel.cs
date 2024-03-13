using System;
using System.ComponentModel.DataAnnotations;

namespace periodTracker.Models.ViewModels
{
    public class LoginViewModel
    {
            //[Key]
            //public int Id {get; set;}
            [Required(ErrorMessage ="Username is required")]
            public string? Username { get; set; }

            [Required(ErrorMessage = "Password is required")]
            [DataType(DataType.Password)]
            public string? Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        

    }
}

