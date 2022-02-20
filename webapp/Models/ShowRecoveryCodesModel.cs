using Microsoft.AspNetCore.Mvc;

namespace webapp.Models
{
    public class ShowRecoveryCodesModel
    {

        [TempData]
        public string[] RecoveryCodes { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

    }
}