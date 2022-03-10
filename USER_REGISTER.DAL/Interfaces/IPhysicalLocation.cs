using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USER_REGISTER.DAL.Interfaces
{
    public interface IPhysicalLocation
    {
        string LocationDetails { get; set; }
        string StreetOrRoad { get; set; }
        string City { get; set; }
        string ContactPerson { get; set; }
        string ContactTelephone1 { get; set; }
        string ContactTelephone2 { get; set; }
        string ContactEmail { get; set; }
    }
}
