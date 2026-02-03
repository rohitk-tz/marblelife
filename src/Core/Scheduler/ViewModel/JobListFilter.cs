using Core.Application.Attribute;
using System;
using System.Collections.Generic;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class JobListFilter
    {
        public bool IsFromScheduler { get; set; }
        public string Text { get; set; }
        public long FranchiseeId { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public long JobTypeId { get; set; }
        public long TechId { get; set; }
        //public bool IsExecutive { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string PropName { get; set; }
        public long? Order { get; set; }
        public IEnumerable<long> ResourceIds { get; set; }
        public int? Option { get; set; }
        public int? Imported { get; set; }
        public bool IsCalendar { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? StartDateForCal { get; set; }
        public DateTime? EndDateForCal { get; set; }
        public bool? Status { get; set; }
        public string Role { get; set; }
        public bool? IsLock { get; set; }
        public long? RoleId { get; set; }
        public long? IsEmpty { get; set; }
        public long? PersonId{ get; set; }
        public string CustomerName { get; set; }
        public long LoggedInUserOrgId{ get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
    }
}
