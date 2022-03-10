using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USER_REGISTER.BLL.Utils
{
    public interface IEmailSender
    {
        (bool, string) SendEmail(string[] addresses, string subject, string body);
    }
}
