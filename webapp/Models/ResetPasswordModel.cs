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
            [RegularExpression(@"^((?=.*[A-Z])(?=.*\d)(?=.*[a-z])|(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])|(?=.*[A-Z])(?=.*[a-z])(?=.*[!@#$%^&*])|(?=.*\d)(?=.*[a-z])(?=.*[!@#$%^&*-])).{15,}$", ErrorMessage = "Passwords must contain at least 1 of each the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*) and must be at least 15 characters.")]
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