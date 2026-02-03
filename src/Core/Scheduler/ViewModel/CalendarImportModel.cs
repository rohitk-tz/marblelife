using Core.Application.Attribute;
using Core.Application.ViewModel;
using System;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class CalendarImportModel : EditModelBase
    {
        public long Id { get; set; }
        public long? TechId { get; set; }
        public long? SalesRepId { get; set; }
        public long FranchiseeId { get; set; }
        public long FileId { get; set; }
        public long TypeId { get; set; }
        public long StatusId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public FileModel File { get; set; }
        public FeedbackMessageModel Message { get; set; }
        public int FailedRecords { get; set; }
        public int SavedRecords { get; set; }
        public int TotalRecords { get; set; }
        public long TimeZoneId { get; set; }
    }
}
