using System;
using Core.Application.Exceptions;

namespace Api.Impl.Exceptions
{
    public class NotAuthenticatedException : CustomBaseException
    {
        public NotAuthenticatedException(): base("Session Expired. Please Login again.")
        {
            
        }

        public NotAuthenticatedException(string message)
            : base(message)
        {

        }
    }
}