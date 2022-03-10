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
    public class EditUserModel : BasePageModel
    {
        private readonly IDbRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISessionService _sessionservice;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _httpContextAccessor;
        [BindProperty]
        public ApplicationUser user { get; set; }
        public EditUserModel(IDbRepository repository, UserManager<ApplicationUser> userManager,
            ISessionService sessionservice, IEmailSender emailSender, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _userManager = userManager;
            _sessionservice = sessionservice;
            _emailSender = emailSender;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult OnGet(string Id)
        {
            user = _repository.Set<ApplicationUser>().Where(r => r.Id == Id).SingleOrDefault() ?? new ApplicationUser();
            return Page();
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
            var dbUser = _repository.Set<ApplicationUser>().Where(r => r.Id == user.Id).SingleOrDefault();

            dbUser.UserName = user.UserName;
            dbUser.Email = user.UserEmail;
            dbUser.LoginToken = Token;
            dbUser.Age = user.Age;
            dbUser.HIVStatus = user.HIVStatus;

            //Attempt to create User Account         
            _repository.SaveChanges();

            return new JsonResult(new
            {
                success,
                errorMessage
            });
        }
    }
}
