using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace USER_REGISTER.Controllers.api
{
    public class AuthenticateUserModel
    {
        public string UsernameOrEmail { get; set; }
        public string Password { get; set; }
    }
}
