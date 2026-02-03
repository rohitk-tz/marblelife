using System;

namespace Core.Application.Exceptions
{
    public abstract class CustomBaseException : Exception
    {
        public CustomBaseException()
        {

        }

        public CustomBaseException(string message) : base(message)
        {

        }
    }
}
