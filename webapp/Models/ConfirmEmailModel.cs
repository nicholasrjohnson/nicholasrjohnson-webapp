using Microsoft.AspNetCore.Mvc;

namespace webapp.Models
{
    public class ConfirmEmailModel
    {
        [TempData]
        public string StatusMessage { get; set; } 
    }
}