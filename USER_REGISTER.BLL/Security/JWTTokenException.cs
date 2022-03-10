using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USER_REGISTER.BLL.Security
{
    public class JWTTokenException : Exception
    {
        public JWTTokenException() : base() { }
        public JWTTokenException(string message) : base(message) { }
    }
}
