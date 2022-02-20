using Microsoft.AspNetCore.Mvc;

namespace webapp.Models
{
    public class ResetAuthenticatorModel
    {
        [TempData]
        public string StatusMessage { get; set; }
    }
}