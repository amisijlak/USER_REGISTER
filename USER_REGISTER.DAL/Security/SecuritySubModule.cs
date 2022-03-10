using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USER_REGISTER.DAL.Security
{
    public enum SecuritySubModule
    {
        //security under 1000
        [Description("System Users")]
        SystemUsers = 2,
        [Description("Security Roles")]
        SecurityRoles = 3
    }
}

