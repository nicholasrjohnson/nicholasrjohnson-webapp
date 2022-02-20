using Microsoft.AspNetCore.Mvc;

namespace webapp.Models
{
    public class TwoFactorAuthenticationModel
    {
         public bool HasAuthenticator { get; set; }

        public int RecoveryCodesLeft { get; set; }

        [BindProperty]
        public bool Is2faEnabled { get; set; }

        public bool IsMachineRemembered { get; set; }

        [TempData]
        public string StatusMessage { get; set; }


    }
}