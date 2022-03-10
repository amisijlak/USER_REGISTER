using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using DNTCaptcha.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using USER_REGISTER.BLL.Data;
using USER_REGISTER.BLL.Security;
using USER_REGISTER.BLL.Utils;
using USER_REGISTER.DAL.Security;
using USER_REGISTER.Helpers;

namespace USER_REGISTER.Pages.Account
{
    [AllowAnonymous]
    public class UserRegisterModel : BasePageModel
    {
        //private readonly IUserRegisterService _UserRegister;
        private readonly IDbRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISessionService _sessionservice;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDNTCaptchaValidatorService _validatorService;
        private readonly DNTCaptchaOptions _captchaOptions;
        public UserRegisterModel(IDbRepository repository, UserManager<ApplicationUser> userManager,
            ISessionService sessionservice, IEmailSender emailSender, IHttpContextAccessor httpContextAccessor, IDNTCaptchaValidatorService validatorService,
            IOptions<DNTCaptchaOptions> options)
        {
            _repository = _repository;
            _userManager = userManager;
            _sessionservice = sessionservice;
            _emailSender = emailSender;
            _httpContextAccessor = httpContextAccessor;
            _validatorService = validatorService;
            _captchaOptions = options == null ? throw new ArgumentNullException(nameof(options)) : options.Value;
        }

        [BindProperty]
        public ApplicationUser user { get; set; }
        public void OnGet()
        {
        }
        public JsonResult OnPost()
        {
            //var (success, errorMessage) = _UserRegister.SaveUser(user, _userManager);
            string errorMessage = "";
            string SuccessMessage = "";
            bool success = false;
            string returnUrl = null;
            string Token = "";

            if (!_validatorService.HasRequestValidCaptchaEntry(Language.English, DisplayMode.ShowDigits))
            {
                this.ModelState.AddModelError(_captchaOptions.CaptchaComponent.CaptchaInputName, "Please Enter Valid Captcha.");
                if (ModelState.ErrorCount > 0)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        errorMessage = "Please Enter Valid Captcha."
                    });
                }
            }

            if (user.PassWord != user.ConfirmPassWord)
            {
                errorMessage = "The passwords entered do not match"; ;
                success = false;
                return new JsonResult(new
                {
                    success,
                    errorMessage
                });
            }
            Random TockenGenerator = new Random();
            Token = TockenGenerator.Next(0, 999999).ToString("D6");

            user.UserName = user.PhoneNumber;
            user.Id = Guid.NewGuid().ToString();
            user.Email = user.UserEmail;
            user.LoginToken = Token;


            //Attempt to create User Account         
            IdentityResult result = _userManager.CreateAsync(user, user.PassWord).Result;
            if (result.Succeeded)
            {
                _userManager.AddToRoleAsync(user, "UNAIDSUser").Wait();
                success = true;
                if (!string.IsNullOrEmpty(user.Email))
                {
                    string confirmationToken = _userManager.
                         GenerateEmailConfirmationTokenAsync(user).Result;
                    var userId = _userManager.GetUserIdAsync(user);

                    var link = Url.Page(
                    "/Account/UserRegister",
                     "ConfirmEmail",
                    values: new { userId = user.Id, token = confirmationToken },
                    protocol: Request.Scheme) +"\n The code is "+ Token;

                    string[] email = new string[] { user.Email };

                    var emailResponse = _emailSender.SendEmail(email, "Confirm your email", link);
                    //EmailHelper emailHelper = new EmailHelper();
                    // bool emailResponse = emailHelper.SendEmail(user.Email, link);

                    success = emailResponse.Item1;
                    errorMessage = emailResponse.Item2;

                    if (emailResponse.Item1)
                        SuccessMessage = "Confirmation sent to your email. Please go to your email and complete the registration.";
                    return new JsonResult(new
                    {
                        success,
                        SuccessMessage
                    });
                }
            }
            else
            {
                errorMessage = $"User Creation Error(s): {string.Join(",", result.Errors.Select(r => $"{r.Code}: {r.Description}"))}";

            }

            
            return new JsonResult(new
            {
                success,
                errorMessage
            });


        }

        public ActionResult OnGetConfirmEmail(string userid, string token)
        {
            string errorMessage = "";
            bool success = false;
            ApplicationUser user = _userManager.FindByIdAsync(userid).Result;
            IdentityResult result = _userManager.
                        ConfirmEmailAsync(user, token).Result;

            if (result.Succeeded)
            {

                return Redirect(GlobalPageLinks.AccountLogin);          

                //errorMessage = "Email confirmed successfully! You can now login and enter your business profile";
                //success = true;
                //return new JsonResult(new
                //{                   
                //    //success,
                //    //errorMessage
                //});
            }
            else
            {
                errorMessage = "Error while confirming your email!";
                return new JsonResult(new
                {
                    success,
                    errorMessage
                });
            }


        }
    }
}
