using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Exceptions
{
    public class InvalidAddressException : CustomBaseException
    {
        public InvalidAddressException(string message)
            : base(message)
        {
        }
    }
}
