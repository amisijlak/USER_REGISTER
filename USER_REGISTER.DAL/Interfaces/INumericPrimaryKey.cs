using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USER_REGISTER.DAL.Interfaces
{
    public interface INumericPrimaryKey : IPrimaryKeyEnabled<long>
    {
    }
    public interface IPrimaryKeyEnabled<T> : IUSER_REGISTERModel
    {
        T Id { get; set; }
    }

    public interface IUSER_REGISTERModel
    {

    }
}
