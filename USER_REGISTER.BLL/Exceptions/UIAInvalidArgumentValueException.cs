using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USER_REGISTER.BLL.Exceptions
{
    public class USER_REGISTERInvalidArgumentValueException : USER_REGISTERException
    {
        public const string DefaultMessage = "Invalid Argument Value!";
        public USER_REGISTERInvalidArgumentValueException(string ErrorMessage) : base(ErrorMessage) { }

        /// <summary>
        /// Default message (<see cref="DefaultMessage"/>) will be thrown.
        /// </summary>
        public USER_REGISTERInvalidArgumentValueException() : base(DefaultMessage) { }
    }
}
