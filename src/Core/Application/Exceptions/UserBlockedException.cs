namespace Core.Application.Exceptions
{
    public class UserBlockedException : CustomBaseException
    {
        public UserBlockedException() : base("User has been blocked.")
        {

        }

        public UserBlockedException(string message)
            : base(message)
        {

        }
    }
}
