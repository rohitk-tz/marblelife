namespace Core.Application.ValueType
{
    public class SecureHash
    {
        public string HashedText { get; set; }
        public string Salt { get; set; }

        public SecureHash(string hashedText, string salt)
        {
            HashedText = hashedText;
            Salt = salt;
        }
    }
}
