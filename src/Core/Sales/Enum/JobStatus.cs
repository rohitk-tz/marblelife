using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Sales.Enum
{
    public enum JobStatusEnum
    {
        Created = 1,
        Assigned = 2,
        InProgress = 3,
        Completed = 4,
        Canceled =5,
        Tentative = 6,
    }
}
