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
    public class SystemUsersModel : BasePageModel
    {
        private readonly IDbRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISecurityService _securityService;
        public List<UserDetails> user { get; set; }
        public SystemUsersModel(IDbRepository repository, ISecurityService _securityService, UserManager<ApplicationUser> userManager)
        {
            _repository = _repository;
            _userManager = userManager;
            this._securityService = _securityService;
        }

        public void OnGet()
        {
            NewUserViewModel Data = new NewUserViewModel();
            Data.LoadDetails(_securityService);
            ////return Page();
            user = Data.users;
        }
    }
}
