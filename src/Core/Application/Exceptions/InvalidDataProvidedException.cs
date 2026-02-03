using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Exceptions
{
    public class InvalidDataProvidedException : CustomBaseException
    {
        public InvalidDataProvidedException()
            : base()
        {
        }
        public InvalidDataProvidedException(string message)
            : base(message)
        {
        }
    }
}
