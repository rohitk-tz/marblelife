using Core.Application.Attribute;
using Core.Application.ViewModel;
using System;
using System.Collections.Generic;

namespace Core.MarketingLead.ViewModel
{
    [NoValidatorRequired]
   public class CallDetailsReportNotesViewModel
    {
        public long? UserId { get; set; }
        public DateTime? Timestamp { get; set; }
        public string CallerId { get; set; }
        public string CallNote { get; set; }
        public long? PreferredContactNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Office { get; set; }
        public string ZipCode { get; set; }
        public string ResultingAction { get; set; }
        public string HouseNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string UserRole { get; set; }
        public string Email { get; set; }
    }

    public class CallDetailsReportNotesListViewModel
    {
        public CallDetailsReportNotesListViewModel()
        {
            CallDetailsReportNotesHistory = new List<CallDetailsReportNotesHistoryModel>();
        }
        public List<CallDetailsReportNotesHistoryModel> CallDetailsReportNotesHistory { get; set; }
        public CallDetailsReportNotesListViewModel Collection { get; set; }
        public CallDetailNotesFilter Filter { get; set; }
        public PagingModel PagingModel { get; set; }
    }

    public class CallDetailsReportNotesHistoryModel
    {
        public long? CallId { get; set; }
        public long Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CallerId { get; set; }
        public string CallNote { get; set; }
        public bool CallMatched { get; set; }
        public string UserRole { get; set; }
        public bool IsExpend { get; set; }
        public long? MarketingLeadId { get; set; }
        public string Office { get; set; }
        public string ResultingAction { get; set; }
        public long? PreferredContactNumber { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string ZipCode { get; set; }
        public string HouseNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public bool IsEdited { get; set; }
    }

    public class CountyDropdownListItem
    {
        public string Display { get; set; }
        public string Value { get; set; }
        public string Alias { get; set; }
        public long Order { get; set; }
        public bool? IsPerpetuity { get; set; }
        public CountyDropdownListItem()
        {
        }

        public CountyDropdownListItem(string text, string value)
        {
            Display = text;
            Value = value;
        }
        public CountyDropdownListItem(string text, string value, string alias)
        {
            Display = text;
            Value = value;
            Alias = alias;
        }
        public CountyDropdownListItem(string text, long id, string alias)
        {
            Display = text;
            Alias = alias;
        }
    }

    public class FranchiseeDropdownListItem
    {
        public string Display { get; set; }
        public string Value { get; set; }
        public bool IsActive { get; set; }
    }

    [NoValidatorRequired]
    public class EditCallDetailsReportNotesViewModel
    {
        public long Id { get; set; }
        public long? UserId { get; set; }
        public DateTime? Timestamp { get; set; }
        public string CallerId { get; set; }
        public string CallNote { get; set; }
        public long? PreferredContactNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Office { get; set; }
        public string ZipCode { get; set; }
        public string ResultingAction { get; set; }
        public string HouseNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string UserRole { get; set; }
        public string Email { get; set; }
    }

}

