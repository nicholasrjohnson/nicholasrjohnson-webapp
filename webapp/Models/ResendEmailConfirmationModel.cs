using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace webapp.Models
{
    public class ResendEmailConfirmationModel
    {
        public ResendEmailConfirmationModel() {
            this.Input = new InputModel();
        }

        public bool ValidSubmit { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

    }
}