using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace webapp.Models
{
    public class ResetPasswordModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

        public ResetPasswordModel() {
            this.Input = new InputModel();
        }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public string Code { get; set; }
        }

    }
}