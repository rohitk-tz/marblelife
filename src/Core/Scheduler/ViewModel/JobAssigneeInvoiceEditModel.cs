using Core.Application.Attribute;
using Core.Application.ViewModel;
using System;
using System.Collections.Generic;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class JobAssigneeInvoiceEditModel : EditModelBase
    {
        public long? AssigneeId { get; set; }
        
        public IEnumerable<long> InvoiceNumbers { get; set; }
    }
}
