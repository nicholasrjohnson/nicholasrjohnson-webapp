using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace webapp.Models
{
    public class ChangeEmailModel
    {
        public string Username { get; set; }

        public string Email { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public ChangeEmailModel() { 
            this.Input = new InputModel();
        }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "New email")]
            public string NewEmail { get; set; }
        }
    }
}