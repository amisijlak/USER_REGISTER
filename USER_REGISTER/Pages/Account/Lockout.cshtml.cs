using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using USER_REGISTER.BLL.Security;

namespace USER_REGISTER.Pages.Account
{
    [AllowAnonymous]
    public class LockoutModel : BasePageModel
    {
        private readonly ISecurityService _securityService;

        public LockoutModel(ISecurityService securityService)
        {
            _securityService = securityService;
        }

        public async Task OnGet()
        {
            if (User.Identity.IsAuthenticated) await _securityService.LogoutAsync();
        }
    }
}
