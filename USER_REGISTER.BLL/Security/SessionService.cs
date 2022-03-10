using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USER_REGISTER.BLL.Data;
using USER_REGISTER.DAL.Security;

namespace USER_REGISTER.BLL.Security
{
    public class SessionService : ISessionService
    {
        private readonly IDbRepository _repository;
        private List<string> SecurityRoleIds = new List<string>();
        private List<SessionServicePermission> Permissions = new List<SessionServicePermission>();

        private ApplicationUser _user;
        private bool _IsInSuperRole;

        public SessionService(IDbRepository repository)
        {
            this._repository = repository;
        }

        public string GetUserId()
        {
            return _user?.Id;
        }

        public void Initialize(JWTTokenDto token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));

            _user = _repository.Set<ApplicationUser>().Where(r => r.Id == token.Id).SingleOrDefault();
            if (_user == null) throw new JWTTokenException("User Not Found!");
            else if (!_user.IsLockedOut())
            {
                throw new Exception(ISecurityService.LockedOutErrorMessage);
            }

            if (_user.LastPasswordChangeDate != token.LastPasswordChangeDate) throw new JWTTokenException("Your token has expired, please login again.");
        }

        public bool HasAccessToPermission(SecurityModule Module, SecuritySubModule SubModule, SecuritySystemAction Action, bool IgnoreSystemAdminPriviledge = false)
        {
            if (_IsInSuperRole && !IgnoreSystemAdminPriviledge)
            {
                return true;
            }
            else if (Permissions.Any(r => r.Module == Module && r.SubModule == SubModule && r.SystemAction == Action))
            {
                return true;
            }

            return false;
        }

        public class SessionServicePermission : ISecurityPermission
        {
            public long PermissionId { get; set; }
            public bool Enabled { get; set; }
            public SecurityModule Module { get; set; }
            public SecuritySubModule SubModule { get; set; }
            public SecuritySystemAction SystemAction { get; set; }
        }

        public List<object> GetMobilePermissions()
        {
            return Permissions.Where(r => r.Module == SecurityModule.FieldAgent).Select(r => (object)new
            {
                Id = $"{r.SubModule.GetIntValue()}-{r.SystemAction.GetIntValue()}",
                r.SubModule,
                r.SystemAction
            }).Distinct().ToList();
        }

        public ApplicationUser GetUser()
        {
            return _user;
        }
        public void InitializeForApiUser(ApplicationUser user)
        {
            if (user == null) return;

            _user = user;

            InitializeUser();
        }

        private void InitializeUser()
        {
            SecurityRoleIds = _repository.Set<IdentityUserRole<string>>().Where(r => r.UserId == _user.Id).Select(r => r.RoleId).Distinct().ToList();

            _IsInSuperRole = _repository.Set<SecurityRole>().Any(r => SecurityRoleIds.Contains(r.Id) && r.Name == Constants.SuperRoleName);

            Permissions = (from a in _repository.Set<RolePermission>()
                           where SecurityRoleIds.Contains(a.RoleId)
                           select new SessionServicePermission
                           {
                               SystemAction = a.Permission.SystemAction,
                               Module = a.Permission.Module,
                               SubModule = a.Permission.SubModule
                           }).Distinct().ToList();
        }

        public object CastToAPIUser()
        {
            var dbUser = GetUser();

            //if (dbUser.UserType != DAL.Security.UserType.Mobile_User && !dbUser.InSuperRole(_repository))
            //{
            //    throw new Exception("You are NOT a Mobile User!");
            //}

            return new
            {
                dbUser.Id,
                dbUser.UserName,
                dbUser.FirstName,
                dbUser.LastName,
                dbUser.OtherName,
                IsSuperUser = dbUser.IsSuperUser()
            };
        }
    }
}
