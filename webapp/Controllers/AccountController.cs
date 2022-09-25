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
        /// <summary>
        /// Used for XSRF protection when adding external logins.
        /// </summary>
        private const string XsrfKey = "XsrfId";


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

        [HttpGet]
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

        [HttpGet]
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
                ViewBag.Message = "Error changing email.";
                return View(model);
            }

            // In our UI email and user name are one and the same, so when we update the email
            // we need to update the user name.
            var setUserNameResult = await _userManager.SetUserNameAsync(user, email);
            if (!setUserNameResult.Succeeded)
            {
                ViewBag.Message = "Error changing user name.";
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);
            ViewBag.Message = "Thank you for confirming your email change.";
            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
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
                return View(); 
            }
            else
            {
                if (model.ValidSubmit.Equals(true))
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
                else
                {
                    ModelState.AddModelError(string.Empty, "Must verify that you are a human.");
                }
                
                return View();
            }
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
        [Authorize]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied() {
            return View();
        }

        [HttpGet] 
        public IActionResult Lockout() {
            return View();
        }

        [HttpGet]
        public IActionResult InvalidLogin()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ConfirmedEmailRequired()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe)
        {
                if (email == null)
                {
                    email = string.Empty;
                }

                if (password == null)
                {
                    password = string.Empty;
                }

                if (_userManager.Options.SignIn.RequireConfirmedEmail)
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    bool emailConfirmed = false;

                    if (user == null)
                    {
                        return new JsonResult( 
                            new Dictionary<string,string>() { 
                                { "url", "/Account/InvalidLogin" },
                                { "succeeded", "false"}
                            });

                    }

                    emailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

                    if (!emailConfirmed) {
                        return new JsonResult( 
                        new Dictionary<string,string>() { 
                           { "url", "/Account/ConfirmedEmailRequired" },
                           { "succeeded", "false"}
                       });
                    }
                }
                
                IList<AuthenticationScheme> externalLogins  = null;
                string indexUrl = Url.Content("~/Home/Index");

                externalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToArray();
              
                // Clear the existing external cookie to ensure a clean login process
                await _httpContextAccessor.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, false);
                if (result.Succeeded)
                {
                    //Login with cookie
                    var identity = new ClaimsIdentity("www.nicholasrjohnson.cookie", ClaimTypes.Name, ClaimTypes.Role);

                   identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));

                   var principal = new ClaimsPrincipal(identity);

                   var authProperties = new AuthenticationProperties
                   {
                        AllowRefresh = true,
                        ExpiresUtc = DateTimeOffset.Now.AddMinutes(60),
                        IsPersistent = true,
                   };

                    await _httpContextAccessor.HttpContext.SignInAsync("www.nicholasrjohnson.scheme", new ClaimsPrincipal(principal),authProperties);

                    _logger.LogInformation("User logged in.");
                    
                     return new JsonResult( 
                         new Dictionary<string,string>() { 
                             { "url", "/Members/MembersIndex" },
                             { "succeeded", "true"}
                         });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return new JsonResult( 
                         new Dictionary<string,string>() { 
                             { "url", "/Account/Lockout" },
                             { "succeeded", "false"}
                         });
                 }
                else
                {
                     return new JsonResult( 
                         new Dictionary<string,string>() { 
                             { "url", "/Account/InvalidLogin" },
                             { "succeeded", "false"}
                     });
                  }
            // If we got this far, something failed, redisplay form
        }

        public async Task<IActionResult> Logout() {
            await _signInManager.SignOutAsync();
             await _httpContextAccessor.HttpContext.SignOutAsync(
                "www.nicholasrjohnson.scheme");
            _httpContextAccessor.HttpContext.Session.Clear();
            _logger.LogInformation("User logged out.");
            return new JsonResult( 
            new Dictionary<string,string>() { 
              { "url", "/Home/Index" },
              { "succeeded", "true"}
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = this.Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = this._signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/)");

            var loginInfo = await this._signInManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                this.ModelState.AddModelError(string.Empty, "Error loading external login information.");

                return this.RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await this._signInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, isPersistent: true);

            if (result.Succeeded)
            {
                var email = this.User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).FirstOrDefault();
                if (email != null)
                {
                    var user = await this._userManager.FindByEmailAsync(email);
                    return this.RedirectToAction("Index", "InnerSanctum");
                }
            }
            else if (result.IsLockedOut)
            {
                return this.View("Lockout");
            }
            else if (result.RequiresTwoFactor)
            {
                return this.RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
            }
            else
            {
                var email = loginInfo.Principal.FindFirstValue(ClaimTypes.Email);

                if (email != null)
                {
                    var user = await this._userManager.FindByEmailAsync(email);
                    this.ViewBag.LoginProvider = loginInfo.LoginProvider;

                    var returnModel = new ExternalLoginConfirmationModel { Email = email, Provider = loginInfo.LoginProvider, LoginKey = loginInfo.ProviderKey };

                    await this._userManager.AddLoginAsync(user, loginInfo);
                    await this._signInManager.SignInAsync(user, isPersistent: true);

                    return this.RedirectToAction("MembersIndex", "Members");
                }
            }

            return this.RedirectToAction("Login");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<IActionResult> ExternalLoginEmail(ExternalLoginConfirmationModel model, string returnUrl, string command)
        {
            if (this.ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                ExternalLoginInfo info = await this._signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return this.View("ExternalLoginFailure");
                }

                ApplicationIdentityUser user = await this._userManager.FindByNameAsync(model.LoginName);
                var result = await this._userManager.AddLoginAsync(user, info);
                if (result.Succeeded)
                {
                    await this._signInManager.SignInAsync(user, isPersistent: true);
                    return this.RedirectToAction("MembersIndex", "Members");
                }
            }

            this.ViewBag.ReturnUrl = returnUrl;
            return this.View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ExternalLoginFailure()
        {
            return this.View();
        }

    
        public IActionResult SuccessfulLogout() {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)// && model != null)
            {
                if (model.ValidSubmit.Equals(true))
                {
                    var user = new ApplicationIdentityUser { UserName = model.Input.Email, Email = model.Input.Email };
                    var result = await _userManager.CreateAsync(user, model.Input.Password);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");

                        var userId = await _userManager.GetUserIdAsync(user);

                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Action("ConfirmEmail", "Account", new {code, userId,  email = user.Email, returnUrl = @Url.Content("/Home/Index")}, Request.Scheme);
                        await _emailSender.SendEmailAsync(model.Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        return View("RegisterConfirmation");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Must verify that you are a human.");
                }

                model = new RegisterModel();
                // If we got this far, something failed, redisplay form
                }
            return View(model);
        }

        [AllowAnonymous]
        public IActionResult RegisterConfirmation(RegisterModel model)
        {
            return View();
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
            
            if (model.ValidSubmit.Equals(true))
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new {code, userId,  email = user.Email}, Request.Scheme);
                await _emailSender.SendEmailAsync(
                model.Input.Email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
            } 
            else
            {
                ModelState.AddModelError(string.Empty, "Must verify that you are a human.");
            }

            return View();
        }
    }
}
