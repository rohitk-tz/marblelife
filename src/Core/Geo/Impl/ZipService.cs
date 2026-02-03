using Core.Application;
using Core.Application.Attribute;
using Core.Geo.Domain;
using System.Text.RegularExpressions;

namespace Core.Geo.Impl
{
    [DefaultImplementation]
    public class ZipService : IZipService
    {
        private readonly IRepository<Zip> _zipRepository;

        public ZipService(IUnitOfWork unitOfWork)
        {
            _zipRepository = unitOfWork.Repository<Zip>();
        }
        public Zip GetbyZipCode(string zipCode)
        {
            return _zipRepository.Get(x => x.Code == zipCode);
        }
        public Zip GetbyZipId(long zipId)
        {
            return _zipRepository.Get(x => x.Id == zipId);
        }

        public bool IsUSZipCode(string zipCode)
        {
            string _usZipRegEx = @"^(?=.*[1-9].*)[0-9]{1,5}$";
            bool validZipCode = true;
            if ((!Regex.Match(zipCode, _usZipRegEx).Success))
            {
                validZipCode = false;
            }
            return validZipCode;
        }

    }
}
