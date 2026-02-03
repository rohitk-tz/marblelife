using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.Enum
{
    public enum ConfirmationEnum
    {
        AlreadyConfirmed = 2,
        ErrorInConfirming = 3,
        InvalidId=4,
        PastScheduler=5,
        NotResponded = 216,
        NotConfirmed=217,
        Confirmed=218
    }
}
