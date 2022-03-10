using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USER_REGISTER.DAL.Interfaces
{
    public interface IPersonName
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        string OtherName { get; set; }
    }
}
