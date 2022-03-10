using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using USER_REGISTER.BLL.Data;
using USER_REGISTER.BLL.Security;
using USER_REGISTER.DAL.Security;

namespace USER_REGISTER.Pages.Account
{
    [AllowAnonymous]
    public class LoginConfirmationCodeModel : BasePageModel
    {
        private readonly IDbRepository _repository;
        private readonly ISecurityService _securityService;
        public LoginConfirmationCodeModel(IDbRepository repository, ISecurityService securityService)
        {
            _repository = repository;
            _securityService = securityService;
        }
        [BindProperty]
        public ApplicationUser user { get; set; }
        public void OnGet(string userName, string email)
        {
            UserName = userName;
            Email = email;
        }

        public JsonResult OnPost()
        {         
                //user.LoginToken = user.LoginToken;
                //user.UserName = user.UserName;
                var (success, errorMessage) = _securityService.SaveApplicationUser(user);

                return new JsonResult(new
                {
                    success,
                    errorMessage
                });

            }
    }
}
