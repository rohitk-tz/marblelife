using Core.Application.Attribute;
using Core.Application.ViewModel;
using Core.Geo.ViewModel;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class JobCustomerEditModel : EditModelBase
    {
        public long? AddressId { get; set; }  
        public long EstimateId { get; set; }
        public long CustomerId { get; set; }
        public long CustomerIds { get; set; }
        public string CustomerName { get; set; }
        public string FullAddress { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public AddressEditModel Address { get; set; }

        public JobCustomerEditModel()
        {
            Address = new AddressEditModel();
        }
    }
}
