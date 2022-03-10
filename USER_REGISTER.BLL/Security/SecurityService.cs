using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USER_REGISTER.BLL.Data;
using USER_REGISTER.BLL.Utils;
using USER_REGISTER.DAL.Security;

namespace USER_REGISTER.BLL.Security
{
    public class SecurityService : ISecurityService
    {
        private const string FailedUserLoginErrorMessage = "Invalid Email or Password!";
        private readonly IDbRepository _repository;
        private readonly RoleManager<SecurityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ISessionService _sessionService;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public SecurityService(IDbRepository repository, RoleManager<SecurityRole> roleManager, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager
            , IEmailSender emailSender, ISessionService sessionService, IOptions<AppSettings> appSettings)
        {
            this._repository = repository;
            this._roleManager = roleManager;
            this._userManager = userManager;
            this._emailSender = emailSender;
            this._sessionService = sessionService;
            this._appSettings = appSettings;
            _signInManager = signInManager;
        }

        public ApplicationUser GetUserByUsernameOrEmail(string usernameOrEmail)
        {
            return _repository.Set<ApplicationUser>().Where(r => r.Email == usernameOrEmail).SingleOrDefault() ??
                _repository.Set<ApplicationUser>().Where(r => r.UserName == usernameOrEmail).SingleOrDefault();
        }

        public async Task<SignInResult> AuthenticateUserAsync(string usernameOrEmail, string password)
        {
            return await AuthenticateUserAsync(GetUserByUsernameOrEmail(usernameOrEmail), password);
        }

        public async Task<SignInResult> AuthenticateUserAsync(ApplicationUser user, string password)
        {
            return user == null ? new SignInResult() : await _signInManager.PasswordSignInAsync(user, password, false, true);
        }

        public async Task<AuthenticatedUserModel> AuthenticateApiUserAsync(string userEmail, string password)
        {
            if (string.IsNullOrEmpty(userEmail)) throw new HttpStatusException(System.Net.HttpStatusCode.Unauthorized, "The Username/Email is required!");
            if (string.IsNullOrEmpty(password)) throw new HttpStatusException(System.Net.HttpStatusCode.Unauthorized, "The Password is required!");

            var dbUser = GetUserByUsernameOrEmail(userEmail);

            var authResult = await AuthenticateUserAsync(dbUser, password);

            //if authentication fails for any reason, no need to assist a hacker in knowing what exactly to change.
            if (!authResult.Succeeded || authResult.IsLockedOut) throw new HttpStatusException(System.Net.HttpStatusCode.Unauthorized, FailedUserLoginErrorMessage);

            //var userRole = _repository.Set<SecurityRole>().Find(dbUser.RoleId);

            return new AuthenticatedUserModel
            {
                Email = dbUser.Email,
                FirstName = dbUser.FirstName,
                Id = dbUser.Id,
                IsSuperUser = dbUser.IsSuperUser(),
                UserName = dbUser.UserName,
                LastName = dbUser.LastName,
                OtherName = dbUser.OtherName,
                //Permissions = userRole?.Permissions.Select(r => r.Permission).ToList() ?? new List<SecurityPermission>()
            };
        }

        /// <summary>
        /// Get the JWT security key from the appsettings
        /// </summary>
        /// <returns></returns>
        public virtual string GetJWTSecurityKeyFromAppSettings()
        {
            return _appSettings.Value.Secret;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public (bool, string) SaveApplicationUser(ApplicationUser model) //
        {
            string errorMessage = null;
            bool success = false;                      


            try
            {
                _repository.ExecuteInNewTransaction(() =>
                {
                    var dbSet = _repository.Set<ApplicationUser>().Where(r=>(r.UserName==model.UserName ||r.Email==model.Email) && r.LoginToken==model.LoginToken).FirstOrDefault();
                    if (dbSet != null)
                    {
                        dbSet.EmailConfirmed = true;
                        _repository.SaveChanges();
                        success = true;
                    }
                    else
                    {
                        errorMessage = "The Code entered ("+model.LoginToken +") is wrong, please check it first.";
                    }
                    
                });

                
            }
            catch (Exception e)
            {
                errorMessage = e.ExtractInnerExceptionMessage();
            }

            return (success, errorMessage);
        }


        public List<ApplicationUser> GetAllUsers()
        {
            var dbSet = _repository.Set<ApplicationUser>();
            return dbSet.ToList();
        }
    }
}
