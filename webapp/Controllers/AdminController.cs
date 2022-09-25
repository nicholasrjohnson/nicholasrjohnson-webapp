using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using webapp.Models;
using webapp.Data;

namespace webapp.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationIdentityUser> _userManager;
        private readonly SignInManager<ApplicationIdentityUser> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;
        private readonly UrlEncoder _urlEncoder;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public AdminController(
            UserManager<ApplicationIdentityUser> userManager,
            SignInManager<ApplicationIdentityUser> signInManager,
            ILogger<ChangePasswordModel> logger,
            IEmailSender emailSender,
            UrlEncoder urlEncoder,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _urlEncoder = urlEncoder;
            _httpContextAccessor = httpContextAccessor;
        } 

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);

            if (hasPassword)
            {
                return View();
            };

            return View("/Views/Admin/AdminIndex.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return View("/Admin/SetPassword");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.Input.OldPassword, model.Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User changed their password successfully.");
            model.StatusMessage = "Your password has been changed.";

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public IActionResult AdminIndex()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SetPassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);

            if (hasPassword)
            {
                return View("/Views/Admin/ChangePassword.cshtml");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ChangeEmail()
        {
            ChangeEmailModel model = await LoadEmailAsync(await _userManager.GetUserAsync(User));
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> SetPassword(SetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, model.Input.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                foreach (var error in addPasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);
            model.StatusMessage = "Your password has been set.";

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DeletePersonalData()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeletePersonalData(DeletePersonalDataModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!await _userManager.CheckPasswordAsync(user, model.Input.Password))
            {
                ModelState.AddModelError(string.Empty, "Incorrect password.");
                return View();
            }

            var result = await _userManager.DeleteAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred deleting user with ID '{userId}'.");
            }

            await _signInManager.SignOutAsync();

            _logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);

            return Redirect("~/");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DownloadPersonalData()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            _logger.LogInformation("User with ID '{UserId}' asked for their personal data.", _userManager.GetUserId(User));

            // Only include personal data for download
            var personalData = new Dictionary<string, string>();
            var personalDataProps = typeof(IdentityUser).GetProperties().Where(
                            prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
            foreach (var p in personalDataProps)
            {
                personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
            }

            var logins = await _userManager.GetLoginsAsync(user);
            foreach (var l in logins)
            {
                personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
            }

            Response.Headers.Add("Content-Disposition", "attachment; filename=PersonalData.json");
            return new FileContentResult(JsonSerializer.SerializeToUtf8Bytes(personalData), "application/json");
        }

        [Authorize]
        private async Task<ChangeEmailModel> LoadEmailAsync(ApplicationIdentityUser user)
        {
            ChangeEmailModel model = new ChangeEmailModel();

            var email = await _userManager.GetEmailAsync(user);
            model.Email = email;

            model.Input = new ChangeEmailModel.InputModel
            {
                NewEmail = email,
            };

            return model;
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> ChangeEmail(ChangeEmailModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                model = await LoadEmailAsync(user);
                return View(model);
            }

            var email = await _userManager.GetEmailAsync(user);
            if (model.Input.NewEmail != email)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, model.Input.NewEmail);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Action("ConfirmEmailChange", "Account", new {code, userId, email = model.Input.NewEmail}, Request.Scheme);
                await _emailSender.SendEmailAsync(
                    model.Input.NewEmail,
                    "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                ViewBag.Message = "Confirmation link to change email sent. Please check your email.";
                return View(model);
            }

            ViewBag.Message = "Your email is unchanged.";
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> SendVerificationEmail()
        {
            ChangeEmailModel model = new ChangeEmailModel();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                model = await LoadEmailAsync(user);
                return View(model);
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new {code, email = user.Email}, Request.Scheme);
            await _emailSender.SendEmailAsync(
                email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            ViewBag.Message = "Verification email sent. Please check your email.";
            return View("~/Views/Admin/ChangeEmail.cshtml", model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ChangePhoneNumber()
        {
            ChangePhoneNumberModel model = new ChangePhoneNumberModel();
            var user = await _userManager.GetUserAsync(User);
            model.Input.PhoneNumber = await _userManager.GetPhoneNumberAsync(user);
            model.Username = await _userManager.GetUserNameAsync(user);
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePhoneNumber(ChangePhoneNumberModel model)
        {  
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/AdminIndex.cshtml");
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (model.Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    model.StatusMessage = "Unexpected error when trying to set phone number.";
                    return View("~/Views/Admin/AdminIndex.cshtml");
                }
                await _signInManager.RefreshSignInAsync(user);
                ViewBag.Message = "Your profile has been updated";
            }

            return View("~/Views/Admin/ChangePhoneNumber.cshtml");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> PersonalData()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return View();
        }
    }
}