using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace webapp.Models
{
    public class ExternalLoginModel
    {
        public ExternalLoginModel() {
            this.Input = new InputModel();
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ProviderDisplayName { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

    }
}