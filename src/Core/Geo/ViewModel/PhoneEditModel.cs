using Core.Application.Attribute;
using Core.Application.ViewModel;

namespace Core.Geo.ViewModel
{
    [NoValidatorRequired]
    public class PhoneEditModel : EditModelBase
    {
        public long Id { get; set; }
        public string PhoneNumber { get; set; }
        public long PhoneType { get; set; }
        public bool IsTransferable { get; set; }
        public long TempId { get; set; }
    }
}
