using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using webapp.Data;

namespace webapp.Models {

    public class ChangePasswordModel 
    {

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public ChangePasswordModel() {
            this.Input = new InputModel();
        }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string OldPassword { get; set; }

            [Required]
            [RegularExpression(@"^((?=.*[A-Z])(?=.*\d)(?=.*[a-z])|(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])|(?=.*[A-Z])(?=.*[a-z])(?=.*[!@#$%^&*])|(?=.*\d)(?=.*[a-z])(?=.*[!@#$%^&*-])).{15,}$", ErrorMessage = "Passwords must contain at least 1 of each the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*) and must be at least 15 characters.")]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }
    }
}