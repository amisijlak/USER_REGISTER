using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USER_REGISTER.DAL.BaseModels;

namespace USER_REGISTER.DAL.Interfaces
{
    public interface IPersonalDetails
    {
        string PhoneNumber { get; set; }
        string Email { get; set; }
        Gender Gender { get; set; }
    }
}
