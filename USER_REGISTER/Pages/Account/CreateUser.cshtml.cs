using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using USER_REGISTER.BLL.Data;
using USER_REGISTER.BLL.Security;
using USER_REGISTER.BLL.Utils;
using USER_REGISTER.DAL.Security;

namespace USER_REGISTER.Pages.Account
{
    public class CreateUserModel : BasePageModel
    {
        private readonly IDbRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISessionService _sessionservice;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CreateUserModel(IDbRepository repository, UserManager<ApplicationUser> userManager,
            ISessionService sessionservice, IEmailSender emailSender, IHttpContextAccessor httpContextAccessor)
        {
            _repository = _repository;
            _userManager = userManager;
            _sessionservice = sessionservice;
            _emailSender = emailSender;
            _httpContextAccessor = httpContextAccessor;
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

            user.UserName = user.UserName;
            user.Id = Guid.NewGuid().ToString();
            user.Email = user.UserEmail;
            user.LoginToken = Token;


            //Attempt to create User Account         
            IdentityResult result = _userManager.CreateAsync(user, user.PassWord).Result;
            if (result.Succeeded)
            {
                _userManager.AddToRoleAsync(user, "SmeUser").Wait();
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
                    protocol: Request.Scheme) + "\n The code is: " + Token +",\n Username: "+ user.UserName +", \n password: "+ user.PassWord;

                    string[] email = new string[] { user.Email };

                    var emailResponse = _emailSender.SendEmail(email, "Confirm your email", link);

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
    }
}
