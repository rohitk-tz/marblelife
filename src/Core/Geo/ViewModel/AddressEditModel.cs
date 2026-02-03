using Core.Application.ViewModel;

namespace Core.Geo.ViewModel
{
    public class AddressEditModel : EditModelBase
    {
        public long Id { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public long StateId { get; set; }
        public string State { get; set; }

        public long CountryId { get; set; }

        public string Country { get; set; }
        public string City { get; set; }

        public long? CityId { get; set; }
        public long? ZipId { get; set; }

        public string ZipCode { get; set; }

        public long AddressType { get; set; }

        public AddressEditModel()
            : this((long)Enum.AddressType.Primary)
        {

        }
        public string FullAddress
        {
            get
            {
                return FullAddressString();
            }
        }
        public string FullAddressString()
        {
            var addrress = string.Empty;
            if (!string.IsNullOrEmpty(AddressLine1))
            {
                addrress += AddressLine1;
            }
            if (!string.IsNullOrEmpty(AddressLine2))
            {
                addrress += " " + AddressLine2;
            }

            return addrress;
        }
        public AddressEditModel(long type)
        {
            AddressType = type;
            StateId = -1;
            CountryId = 1;
        }

        public static bool IsNullOrEmpty(AddressEditModel model)
        {
            return model == null ||
                   (string.IsNullOrEmpty(model.AddressLine1) && string.IsNullOrEmpty(model.AddressLine2) &&
                    string.IsNullOrEmpty(model.City) && string.IsNullOrEmpty(model.ZipCode) && model.StateId < 1);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is AddressEditModel))
                return base.Equals(obj);

            var model = obj as AddressEditModel;

            return model.AddressLine1 == AddressLine1 && model.AddressLine2 == AddressLine2 && model.ZipCode == ZipCode &&
                   model.City == City && model.StateId == StateId && model.CountryId == CountryId;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
