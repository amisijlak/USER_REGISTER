using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USER_REGISTER.DAL.Security;

namespace USER_REGISTER
{
    public interface IProtectActionAttribute
    {
        SecurityModule Module { get; set; }
        SecuritySubModule SubModule { get; set; }
        SecuritySystemAction SystemAction { get; set; }
    }
}
