using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USER_REGISTER.BLL.Exceptions
{
    public class USER_REGISTERException : Exception
    {
        public USER_REGISTERException(string ErrorMessage) : base(ErrorMessage) { }
        public USER_REGISTERException() : base() { }
    }
}
