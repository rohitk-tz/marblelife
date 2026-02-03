using Core.Application.ValueType;

namespace Core.Application
{
    public interface ICryptographyOneWayHashService
    {
        SecureHash CreateHash(string text);
        bool Validate(string text, SecureHash goodHash);
    }
}
