using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DNTCaptcha.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using USER_REGISTER.BLL.Data;
using USER_REGISTER.BLL.Security;
using USER_REGISTER.DAL.Security;

namespace USER_REGISTER.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : BasePageModel
    {
        [BindProperty]
        [Required]
        [Display(Name = "Username/Email")]
        public string UsernameOrEmail { get; set; }
        [BindProperty]
        [Required]
        public string Password { get; set; }
        [BindProperty]
        public string ReturnUrl { get; set; }

        private readonly ISecurityService _securityService; 
        private readonly IDbRepository _repository;
        private readonly IDNTCaptchaValidatorService _validatorService;
        private readonly DNTCaptchaOptions _captchaOptions;

        public LoginModel(ISecurityService securityService, IDbRepository repository, IDNTCaptchaValidatorService validatorService,
            IOptions<DNTCaptchaOptions> options)
        {
            _securityService = securityService;
            _repository = repository;
            _validatorService = validatorService;
            _captchaOptions = options == null ? throw new ArgumentNullException(nameof(options)) : options.Value;
        }
        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPost()
        {
            if (!_validatorService.HasRequestValidCaptchaEntry(Language.English, DisplayMode.ShowDigits))
            {
                this.ModelState.AddModelError(_captchaOptions.CaptchaComponent.CaptchaInputName, "Please Enter Valid Captcha.");
                return RedirectToPage("/Account/Login", new { userName = UsernameOrEmail, email = Email });
            }

            Microsoft.AspNetCore.Identity.SignInResult result = null;

            var Data = _repository.Set<ApplicationUser>().Where(r => r.UserName == UsernameOrEmail || r.Email== UsernameOrEmail);
            if (Data == null)
            {
                ErrorMessage = "Invalid Username/Email Or Password Or Account not confirmed!";
            }
            else
            {
                var Confirmed = Data.Select(r => r.EmailConfirmed).FirstOrDefault();
                if (!Confirmed)
                {
                     
                    ErrorMessage = "Please enter the code sent to your email to complete registration!";
                     UserName = UsernameOrEmail.Contains("@") ? "" : UsernameOrEmail ;
                    Email = UsernameOrEmail.Contains("@") ? UsernameOrEmail : ""; 
                    return RedirectToPage("/Account/LoginConfirmationCode", new { userName = UsernameOrEmail, email = Email }); 
                }
            }

            if (await ExecuteUnsafeCodeAsync(async () =>
            {
                result = await _securityService.AuthenticateUserAsync(UsernameOrEmail, Password);
            }))
            {
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        return Redirect(GlobalPageLinks.AfterLoginIndex);
                    }
                }
                else if (result.IsLockedOut)
                {
                    return Redirect(GlobalPageLinks.AccountLockout);
                }
                else
                {
                    ErrorMessage = "Invalid Username/Email Or Password Or Account not confirmed!";
                }
            }

            return Page();
        }

        
    }
}
