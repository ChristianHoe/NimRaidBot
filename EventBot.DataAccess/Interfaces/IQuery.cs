using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBot.DataAccess.Queries
{
    public interface IQuery<T, R>
    {
        R Execute(T t);
    }
}
