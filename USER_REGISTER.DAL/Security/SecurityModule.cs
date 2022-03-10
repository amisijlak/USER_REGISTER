using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USER_REGISTER.DAL.Security
{
    public enum SecurityModule
    {
        [Description("Field Agent")]
        FieldAgent = 1,
        Security = 2,
        Settings = 3,
        Registration=4,
    }
}
