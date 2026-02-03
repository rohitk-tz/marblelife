using Core.Geo.Domain;

namespace Core.Geo
{
    public interface IZipService
    {
        Zip GetbyZipCode(string zipCode);
        bool IsUSZipCode(string zipCode);
        Zip GetbyZipId(long zipId);
    }
}
