using Core.Application.Attribute;
using Core.Application.ViewModel;
using Core.Geo.ViewModel;
using System;
using System.Collections.Generic;

namespace Core.Sales.ViewModel
{
    [NoValidatorRequired]
    public class CustomerEditModel : EditModelBase
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<EmailEditModel> Emails { get; set; }
        public AddressEditModel Address { get; set; }
        public string PhoneNumber { get; set; }
        public string MarketingClass { get; set; }
        public string Franchisee { get; set; }
        public string ContactPerson { get; set; }
        public long MarketingClassId { get; set; }
        public long? AddressId { get; set; }
        public bool ReceiveNotification { get; set; }
        public DateTime? DateCreated { get; set; }
        public decimal? TotalSales { get; set; }
        public int? NoOfSales { get; set; }  

        public CustomerEditModel()
        {
            Emails = new List<EmailEditModel>();
        }
    }
}
