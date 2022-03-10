using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using USER_REGISTER.BLL.Security;

namespace USER_REGISTER.Pages.Account
{
    public class LogoutModel : BasePageModel
    {
        private readonly ISecurityService _securityService;

        public LogoutModel(ISecurityService securityService)
        {
            _securityService = securityService;
        }

        public async Task<IActionResult> OnGet()
        {
            if (User.Identity.IsAuthenticated) await _securityService.LogoutAsync();

            return Redirect(GlobalPageLinks.Index);
        }
    }
}
