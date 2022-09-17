namespace webapp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public class ExternalLoginConfirmationModel
    {
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
         public string Email { get; set; }

        public int ContactId { get; set; }

        public int ContactUserId { get; set; }

        public string FirstName { get; set; }

        public string Provider { get; set; }

        public string LoginKey { get; set; }

        public string LoginName { get; set; }

        public string Password { get; set; }
    }
}