using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USER_REGISTER.DAL.Interfaces
{
    public interface IGeographicLocation
    {
        long? RegionId { get; set; }
        long? SubRegionId { get; set; }
        long? DistrictId { get; set; }
        long? SubCountyId { get; set; }
        long? ParishId { get; set; }
        string village { get; set; }
    }
}

