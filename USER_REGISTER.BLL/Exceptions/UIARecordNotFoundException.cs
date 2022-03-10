using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USER_REGISTER.BLL.Exceptions
{
    public class USER_REGISTERRecordNotFoundException : USER_REGISTERException
    {
        public const string DefaultMessage = "Record Not Found!";

        public USER_REGISTERRecordNotFoundException(string ErrorMessage) : base(ErrorMessage) { }

        /// <summary>
        /// Default message (<see cref="DefaultMessage"/>) will be thrown.
        /// </summary>
        public USER_REGISTERRecordNotFoundException() : base(DefaultMessage) { }
    }
}
