using Core.Application.Attribute;
using System;


namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class ZipDataListFilter
    {
        public long? StatusId { get; set; }
        public string UploadedBy { get; set; }
        public string SortingColumn { get; set; }
        public long? SortingOrder { get; set; }
        public string text { get; set; }
        public long? CountryId { get; set; }
    }
}
