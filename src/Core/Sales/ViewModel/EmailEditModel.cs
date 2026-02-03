using Core.Application.Attribute;
using Core.Application.ViewModel;
using System;

namespace Core.Sales.ViewModel
{
    [NoValidatorRequired]
    public class EmailEditModel : EditModelBase
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public string email { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsSynced { get; set; } 
    }
}
