using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using webapp.Models;
using webapp.Data;

namespace webapp.Controllers
{
    public class AccountController : Controller
    {       
        private readonly UserManager<ApplicationIdentityUser> _userManager;
        private readonly SignInManager<ApplicationIdentityUser> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public AccountController(
            UserManager<ApplicationIdentityUser> userManager,
            SignInManager<ApplicationIdentityUser> signInManager,
            ILogger<ChangePasswordModel> logger,
            IEmailSender emailSender,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _httpContextAccessor = httpContextAccessor;
        } 

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(string userId, string code) {
            ConfirmEmailModel model = new ConfirmEmailModel();
            if(userId == null || code == null) {
                return View("/Views/Home/Index.cshtml");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            var result = await _userManager.ConfirmEmailAsync(user, code);                        
        

            return View(result.Succeeded ? "/Views/Account/ConfirmEmail.cshtml" : "/Views/Home/Error.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmailChange(string userId, string email, string code)
        {
            ConfirmEmailChangeModel model = new ConfirmEmailChangeModel();

            if (userId == null || email == null || code == null)
            {
                return RedirectToPage("/Views/Home/Index.cshtml");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ChangeEmailAsync(user, email, code);
            if (!result.Succeeded)
            {
                model.StatusMessage = "Error changing email.";
                return View(model);
            }

            // In our UI email and user name are one and the same, so when we update the email
            // we need to update the user name.
            var setUserNameResult = await _userManager.SetUserNameAsync(user, email);
            if (!setUserNameResult.Succeeded)
            {
                model.StatusMessage = "Error changing user name.";
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);
            model.StatusMessage = "Thank you for confirming your email change.";
            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                model = new ForgotPasswordModel();
            }
            else
            {
                var user = await _userManager.FindByEmailAsync(model.Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    return View("/Views/Account/ForgotPasswordConfirmation.cshtml");
                }
                
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Action(nameof(ResetPassword), "Account", new {code, email = user.Email}, Request.Scheme);
                await _emailSender.SendEmailAsync(
                    model.Input.Email,
                    "Reset Password",
                    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                return View("/Views/Account/ForgotPasswordConfirmation.cshtml");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation() {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string code=null)
        {
            ResetPasswordModel model = new ResetPasswordModel();

            if (code == null)
            {
                return BadRequest("A code must be supplied for password reset.");
            }
            else
            {
                model.Input = new ResetPasswordModel.InputModel
                {
                    Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code)),
                    Email = email,
                };
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {

            if (!ModelState.IsValid)
            {
                return View("Views/Home/Index.cshtml");
            }

            var user = await _userManager.FindByEmailAsync(model.Input.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Input.Code, model.Input.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }
            return View();
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        public IActionResult AccessDenied() {
            return View();
        }

        public IActionResult GetExternalLogin()
        {
            return RedirectToPage("./Login");
        } 

        public IActionResult PostExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginModel model, string remoteError = null)
        {

            if (!ModelState.IsValid) { 
                model = new ExternalLoginModel();
                return View(model);
            } else {
                model.ReturnUrl = model.ReturnUrl;

                model.ReturnUrl = model.ReturnUrl ?? Url.Content("~/");
                
                if (model.ErrorMessage != null)
                {
                    model.ErrorMessage = $"Error from external provider: {remoteError}";
                    return RedirectToPage("Login", model);
                }
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    model.ErrorMessage = "Error loading external login information.";
                    return RedirectToPage("Login", model);
                }

                // Sign in the user with this external login provider if the user already has a login.
                var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor : true);
                if (result.Succeeded)
                {
                    _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                    return LocalRedirect(model.ReturnUrl);
                }
                if (result.IsLockedOut)
                {
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    // If the user does not have an account, then ask the user to create an account.
                    model.ProviderDisplayName = info.ProviderDisplayName;
                    if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                    {
                        model.Input = new ExternalLoginModel.InputModel
                        {
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                        };
                    }
                }

        

                // Get the information about the user from the external login provider
                if (info == null)
                {
                    model.ReturnUrl = model.ReturnUrl;
                    model.ErrorMessage = "Error loading external login information during confirmation.";
                    return View("Login", model);
                }
            
                var user = new ApplicationIdentityUser { UserName = model.Input.Email, Email = model.Input.Email };

                var resultCreateUser = await _userManager.CreateAsync(user);
                if (resultCreateUser.Succeeded)
                {
                    resultCreateUser = await _userManager.AddLoginAsync(user, info);
                    if (resultCreateUser.Succeeded)
                    {
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(model.Input.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        // If account confirmation is required, we need to show the link if we don't have a real email sender
                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("./RegisterConfirmation", new { Email = model.Input.Email });
                        }

                        await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);

                        return LocalRedirect(model.ReturnUrl);
                    }
                }
                foreach (var error in resultCreateUser.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                
                model.ProviderDisplayName = info.ProviderDisplayName;
            }
            return View(model);
        }        

        public IActionResult Lockout() {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> LoginBefore(LoginModel model)
        {
            if (!ModelState.IsValid) {
                return View("~/", model);
            }
            else
            {
                model.ReturnUrl = model.ReturnUrl ?? Url.Content("~/");

                model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                if (!string.IsNullOrEmpty(model.ErrorMessage))
                {
                    ModelState.AddModelError(string.Empty, model.ErrorMessage);
                }

                // Clear the existing external cookie to ensure a clean login process
                await _httpContextAccessor.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

                model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Input.Email, model.Input.Password, model.Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return View("~/", model);
                }
                if (result.RequiresTwoFactor)
                {
                    LoginWith2faModel login2fa = new LoginWith2faModel();

                    login2fa.ReturnUrl = model.ReturnUrl;
                    login2fa.RememberMe = model.Input.RememberMe;


                    return View("LoginWith2fa", login2fa);
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View("~/", model);
                }
            }
            // If we got this far, something failed, redisplay form
        }

        public async Task<IActionResult> LogoutBefore(LogoutModel model) {
            if(!ModelState.IsValid) {
                model.ReturnUrl = "~/Account/SuccessfulLogout";
                model = new LogoutModel();
                return View("~/", model);
            }
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            if (model.ReturnUrl != null)
            {
                return LocalRedirect(model.ReturnUrl);
            }
            else
            {
                return View("~/", model);
            }   
        }

        [AllowAnonymous]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe)
        {
                IList<AuthenticationScheme> externalLogins  = null;
                string indexUrl = Url.Content("~/Home/Index");

                externalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToArray();
              
                // Clear the existing external cookie to ensure a clean login process
                await _httpContextAccessor.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

                externalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToArray();
            
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    
                     return new JsonResult( 
                         new Dictionary<string,string>() { 
                             { "url", "" },
                             { "succeeded", "true"}
                         });
                }
                if (result.RequiresTwoFactor)
                {
                    LoginWith2faModel login2fa = new LoginWith2faModel();

                    string returnUrl = "~/Account/LoginWith2fa";


                     return new JsonResult( 
                         new Dictionary<string,string>() { 
                             { "url", returnUrl },
                             { "succeeded", "false"}
                         });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return new JsonResult( 
                         new Dictionary<string,string>() { 
                             { "url", "~/Account/Lockout" },
                             { "succeeded", "false"}
                         });
                 }
                else
                {
            return new JsonResult( 
                         new Dictionary<string,string>() { 
                             { "url", "InvalidLogin" },
                             { "succeeded", "false"}
                         });
                  }
            // If we got this far, something failed, redisplay form
        }

        public async Task<IActionResult> Logout() {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
                        return new JsonResult( 
                         new Dictionary<string,string>() { 
                             { "succeeded", "true"}
                         });

        }
    

        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            model.ReturnUrl = model.ReturnUrl ?? Url.Content("~/");

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            var authenticatorCode = model.Input.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, model.RememberMe, model.Input.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID '{UserId}' logged in with 2fa.", user.Id);
                return LocalRedirect(model.ReturnUrl);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);
                return RedirectToPage("./Lockout");
            }
            else
            {
                _logger.LogWarning("Invalid authenticator code entered for user with ID '{UserId}'.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return View(model);
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> GetLoginWithRecoveryCodeAsync(string returnUrl = null)
        {
            LoginWithRecoveryCodeModel model = new LoginWithRecoveryCodeModel();

            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            model.ReturnUrl = returnUrl;

            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> PostLoginWithRecoveryCode(string returnUrl = null)
        {
            LoginWithRecoveryCodeModel model = new LoginWithRecoveryCodeModel();
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            var recoveryCode = model.Input.RecoveryCode.Replace(" ", string.Empty);

            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID '{UserId}' logged in with a recovery code.", user.Id);
                return LocalRedirect(returnUrl ?? Url.Content("~/"));
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);
                return RedirectToPage("./Lockout");
            }
            else
            {
                _logger.LogWarning("Invalid recovery code entered for user with ID '{UserId}' ", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
                return View(model);
            }
        }

        public IActionResult SuccessfulLogout() {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            model.ReturnUrl = model.ReturnUrl ?? Url.Content("~/");
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)// && model != null)
            {
                var user = new ApplicationIdentityUser { UserName = model.Input.Email, Email = model.Input.Email };
                var result = await _userManager.CreateAsync(user, model.Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = model.ReturnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(model.Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return View("RegisterConfirmation", model);
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(model.ReturnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            model = new RegisterModel();
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> RegisterConfirmation(RegisterModel model)
        {
            if (model.Input.Email == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByEmailAsync(model.Input.Email);
            if (user == null)
            {
                return NotFound($"Unable to load user with email '{model.Input.Email}'.");
            }

            RegisterConfirmationModel registerModel = new RegisterConfirmationModel();

            registerModel.Email = model.Input.Email;

            // Once you add a real email sender, you should remove this code that lets you confirm the account
            registerModel.DisplayConfirmAccountLink = true;
            if (registerModel.DisplayConfirmAccountLink)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
               registerModel.EmailConfirmationUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code, returnUrl = registerModel.EmailConfirmationUrl },
              protocol: Request.Scheme);
            }
            else
            {
                registerModel = null;
            }

            return View(registerModel);
        }

        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _userManager.FindByEmailAsync(model.Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
                return View();
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                model.Input.Email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
            return View();
        }
    }
}
