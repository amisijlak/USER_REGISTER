using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USER_REGISTER.DAL.Security
{
    public enum SecuritySystemAction
    {
        //generic actions under 1000
        [Description("Create and Edit")]
        CreateAndEdit = 1,
        Delete = 2,
        [Description("View List")]
        ViewList = 3,
        Review = 10,

        //security specific actions 1000-9999
        Login = 1004,
        LogOff = 1005,
        [Description("Change Password")]
        ChangePassword = 1006,
        [Description("Reset Password")]
        ResetPassword = 1007,
        Activate = 1008,
        Deactivate = 1009,
    }
}
