using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Exceptions
{
   public class InvalidFileUploadException : CustomBaseException
    {
        public InvalidFileUploadException(): base("Invalid File Upload. There should be only 2 tabs in GeoCode File")
        {

        }
        public InvalidFileUploadException(string message)
            : base(message)
        {

        }
    }
}
