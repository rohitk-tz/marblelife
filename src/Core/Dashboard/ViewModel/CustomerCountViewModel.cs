using Core.Application.Attribute;

namespace Core.Dashboard.ViewModel
{
    [NoValidatorRequired]
    public class CustomerCountViewModel
    {
        public long TotalCustomers { get; set; }
        public long CustomerWithEmails { get; set; }
        public long CustomerWithAddress { get; set; }

    }
}
