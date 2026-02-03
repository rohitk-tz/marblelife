using Core.Application.Attribute;
using System;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class JobInfoEditModel
    {
        public long Id { get; set; }
        public long? JobId { get; set; }
        public long? AssigneeId { get; set; }
        public string Assignee { get; set; }
        public string Title { get; set; }
        public string Franchisee { get; set; }
        public long FranchiseeId { get; set; }
        public string CustomerClass { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public DateTime? JobStart { get; set; }
        public DateTime? JobEnd { get; set; }
        public string SalesRep { get; set; }
        public JobCustomerEditModel JobCustomer { get; set; }
        public string GeoCode { get; set; }
        public bool? IsCustomerMailSend { get; set; }
    }
}
