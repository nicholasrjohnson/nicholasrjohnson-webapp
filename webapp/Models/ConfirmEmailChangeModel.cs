using Microsoft.AspNetCore.Mvc;

namespace webapp.Models
{
    public class ConfirmEmailChangeModel
    {
       
        [TempData]
        public string StatusMessage { get; set; } 
    }
}