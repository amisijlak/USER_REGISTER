using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USER_REGISTER.DAL.Security;

namespace USER_REGISTER.BLL.Security
{
    public interface ISessionService
    {
        string GetUserId();
        //string GetUserName();
        //bool IsSuperAdmin();
        //bool IsInSuperRole();
        //ApplicationUser GetUser();
        //List<string> GetSecurityRoleIds();

        //string GetIPAddress();

        //long? _GetCookieValue(string CookieKey);
        //void DestroyCookies();

        //void SetProtectedAction(SecurityModule Module, SecuritySubModule SubModule, SecuritySystemAction Action);
        bool HasAccessToPermission(SecurityModule Module, SecuritySubModule SubModule, SecuritySystemAction Action, bool IgnoreSystemAdminPriviledge = false);
        /// <summary>
        /// Initialize the user
        /// </summary>
        /// <param name="_user"></param>
        void InitializeForApiUser(ApplicationUser _user);

        /// <summary>
        /// Initialize the User currently logged in.
        /// </summary>
        /// <param name="userId"></param>
        void Initialize(JWTTokenDto token);
        List<object> GetMobilePermissions();
        object CastToAPIUser();
    }
}
