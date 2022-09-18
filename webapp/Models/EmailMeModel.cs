namespace webapp.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class EmailMeModel
    {
        public bool ValidSubmit { get; set; }

        [Required]
        [Display(Name = "Name")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "E-mail Address")]
        [StringLength(100, ErrorMessage = "The email address cannot be longer than 50 characters")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "The email address must contain an 'at' symbol (@) and at least one period (.)")]
        public string Email { get; set; }

       [Required]
       [StringLength(1000, ErrorMessage = "The message is too long")]
       public string Message { get; set; }
    }
}